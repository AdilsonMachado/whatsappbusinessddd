using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace WhatsAppBusiness.Infrastructure.WhatsApp;

/// <summary>
/// HTTP client for WhatsApp Business API
/// </summary>
public class WhatsAppClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WhatsAppClient> _logger;
    private const string ApiVersion = "v21.0";

    public WhatsAppClient(HttpClient httpClient, ILogger<WhatsAppClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sends a text message to a WhatsApp user
    /// </summary>
    /// <param name="phoneNumberId">WhatsApp Phone Number ID</param>
    /// <param name="to">Recipient phone number</param>
    /// <param name="message">Message text</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>WhatsApp message ID</returns>
    public async Task<string> SendMessageAsync(
        string phoneNumberId,
        string to,
        string message,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            messaging_product = "whatsapp",
            to,
            type = "text",
            text = new { body = message }
        };

        var response = await SendRequestAsync(phoneNumberId, payload, cancellationToken);
        return ExtractMessageId(response);
    }

    /// <summary>
    /// Sends a template message to a WhatsApp user
    /// </summary>
    /// <param name="phoneNumberId">WhatsApp Phone Number ID</param>
    /// <param name="to">Recipient phone number</param>
    /// <param name="templateName">Template name</param>
    /// <param name="languageCode">Language code (e.g., "en_US")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>WhatsApp message ID</returns>
    public async Task<string> SendTemplateMessageAsync(
        string phoneNumberId,
        string to,
        string templateName,
        string languageCode = "en_US",
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            messaging_product = "whatsapp",
            to,
            type = "template",
            template = new
            {
                name = templateName,
                language = new { code = languageCode }
            }
        };

        var response = await SendRequestAsync(phoneNumberId, payload, cancellationToken);
        return ExtractMessageId(response);
    }

    /// <summary>
    /// Marks a message as read
    /// </summary>
    /// <param name="phoneNumberId">WhatsApp Phone Number ID</param>
    /// <param name="messageId">WhatsApp message ID to mark as read</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task MarkAsReadAsync(
        string phoneNumberId,
        string messageId,
        CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            messaging_product = "whatsapp",
            status = "read",
            message_id = messageId
        };

        await SendRequestAsync(phoneNumberId, payload, cancellationToken);
    }

    private async Task<string> SendRequestAsync(
        string phoneNumberId,
        object payload,
        CancellationToken cancellationToken)
    {
        var url = $"/{ApiVersion}/{phoneNumberId}/messages";
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogDebug("Sending WhatsApp API request to {Url}: {Payload}", url, json);

        var response = await _httpClient.PostAsync(url, content, cancellationToken);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("WhatsApp API request failed with status {StatusCode}: {Response}",
                response.StatusCode, responseContent);
            throw new HttpRequestException(
                $"WhatsApp API request failed with status {response.StatusCode}: {responseContent}");
        }

        _logger.LogDebug("WhatsApp API response: {Response}", responseContent);
        return responseContent;
    }

    private static string ExtractMessageId(string response)
    {
        using var document = JsonDocument.Parse(response);
        var messages = document.RootElement.GetProperty("messages");
        if (messages.GetArrayLength() > 0)
        {
            return messages[0].GetProperty("id").GetString() ?? string.Empty;
        }

        throw new InvalidOperationException("Failed to extract message ID from WhatsApp response");
    }
}
