using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using WhatsAppBusiness.Application.UseCases.Commands;

namespace WhatsAppBusiness.WebAPI.Webhooks;

/// <summary>
/// Controller for handling WhatsApp webhook callbacks
/// </summary>
[ApiController]
[Route("api/webhook/whatsapp")]
public class WhatsAppWebhookController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WhatsAppWebhookController> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the WhatsAppWebhookController
    /// </summary>
    public WhatsAppWebhookController(
        IMediator mediator,
        ILogger<WhatsAppWebhookController> logger,
        IConfiguration configuration)
    {
        _mediator = mediator;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Webhook verification endpoint (GET)
    /// </summary>
    /// <param name="hub_mode">Hub mode</param>
    /// <param name="hub_verify_token">Verify token from WhatsApp</param>
    /// <param name="hub_challenge">Challenge string to echo back</param>
    /// <returns>Challenge string if verification succeeds</returns>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult VerifyWebhook(
        [FromQuery(Name = "hub.mode")] string hub_mode,
        [FromQuery(Name = "hub.verify_token")] string hub_verify_token,
        [FromQuery(Name = "hub.challenge")] string hub_challenge)
    {
        _logger.LogInformation("Webhook verification request received");

        var verifyToken = _configuration["WhatsApp:VerifyToken"];

        if (string.IsNullOrEmpty(verifyToken))
        {
            _logger.LogError("Verify token not configured");
            return StatusCode(500, "Verify token not configured");
        }

        if (hub_mode == "subscribe" && hub_verify_token == verifyToken)
        {
            _logger.LogInformation("Webhook verified successfully");
            return Ok(hub_challenge);
        }

        _logger.LogWarning("Webhook verification failed - invalid token");
        return Forbid();
    }

    /// <summary>
    /// Webhook endpoint for receiving messages from WhatsApp (POST)
    /// </summary>
    /// <param name="payload">The webhook payload</param>
    /// <returns>Success response</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReceiveWebhook([FromBody] WhatsAppWebhookPayload payload)
    {
        _logger.LogInformation("Webhook payload received");

        // Validate signature
        if (!ValidateSignature())
        {
            _logger.LogWarning("Invalid webhook signature");
            return Unauthorized(new { error = "Invalid signature" });
        }

        if (payload?.Entry == null || payload.Entry.Count == 0)
        {
            _logger.LogWarning("Empty webhook payload received");
            return Ok(new { status = "ok" });
        }

        try
        {
            foreach (var entry in payload.Entry)
            {
                if (entry.Changes == null) continue;

                foreach (var change in entry.Changes)
                {
                    if (change.Value?.Messages == null) continue;

                    foreach (var message in change.Value.Messages)
                    {
                        _logger.LogInformation(
                            "Processing message {MessageId} from {From}",
                            message.Id,
                            message.From);

                        var command = new ReceiveWhatsAppMessageCommand
                        {
                            MessageId = message.Id ?? string.Empty,
                            From = message.From ?? string.Empty,
                            Text = message.Text?.Body ?? string.Empty,
                            Type = message.Type ?? "text",
                            Timestamp = message.Timestamp.HasValue
                                ? DateTimeOffset.FromUnixTimeSeconds(message.Timestamp.Value).DateTime
                                : DateTime.UtcNow
                        };

                        var result = await _mediator.Send(command);

                        if (result.IsFailure)
                        {
                            _logger.LogError(
                                "Failed to process message {MessageId}: {Error}",
                                message.Id,
                                result.Error);
                        }
                    }
                }
            }

            return Ok(new { status = "ok" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook payload");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    private bool ValidateSignature()
    {
        // Get the signature from headers
        if (!Request.Headers.TryGetValue("X-Hub-Signature-256", out var signatureHeader))
        {
            _logger.LogWarning("No signature header found");
            return true; // In development, we might skip validation
        }

        var appSecret = _configuration["WhatsApp:AppSecret"];
        if (string.IsNullOrEmpty(appSecret))
        {
            _logger.LogWarning("App secret not configured, skipping signature validation");
            return true; // Skip validation if not configured
        }

        try
        {
            // Read the raw body
            Request.Body.Position = 0;
            using var reader = new StreamReader(Request.Body, leaveOpen: true);
            var body = reader.ReadToEnd();
            Request.Body.Position = 0;

            // Calculate expected signature
            var expectedSignature = CalculateSignature(body, appSecret);
            var signature = signatureHeader.ToString().Replace("sha256=", "");

            return signature == expectedSignature;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating signature");
            return false;
        }
    }

    private static string CalculateSignature(string payload, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(payloadBytes);
        return Convert.ToHexString(hashBytes).ToLower();
    }
}

#region Webhook Models

/// <summary>
/// WhatsApp webhook payload
/// </summary>
public class WhatsAppWebhookPayload
{
    public string? Object { get; set; }
    public List<WebhookEntry>? Entry { get; set; }
}

/// <summary>
/// Webhook entry
/// </summary>
public class WebhookEntry
{
    public string? Id { get; set; }
    public List<WebhookChange>? Changes { get; set; }
}

/// <summary>
/// Webhook change
/// </summary>
public class WebhookChange
{
    public WebhookValue? Value { get; set; }
    public string? Field { get; set; }
}

/// <summary>
/// Webhook value
/// </summary>
public class WebhookValue
{
    public string? MessagingProduct { get; set; }
    public WebhookMetadata? Metadata { get; set; }
    public List<WebhookMessage>? Messages { get; set; }
    public List<WebhookStatus>? Statuses { get; set; }
}

/// <summary>
/// Webhook metadata
/// </summary>
public class WebhookMetadata
{
    public string? DisplayPhoneNumber { get; set; }
    public string? PhoneNumberId { get; set; }
}

/// <summary>
/// Webhook message
/// </summary>
public class WebhookMessage
{
    public string? Id { get; set; }
    public string? From { get; set; }
    public long? Timestamp { get; set; }
    public string? Type { get; set; }
    public WebhookMessageText? Text { get; set; }
}

/// <summary>
/// Webhook message text
/// </summary>
public class WebhookMessageText
{
    public string? Body { get; set; }
}

/// <summary>
/// Webhook status
/// </summary>
public class WebhookStatus
{
    public string? Id { get; set; }
    public string? Status { get; set; }
    public long? Timestamp { get; set; }
    public string? RecipientId { get; set; }
}

#endregion
