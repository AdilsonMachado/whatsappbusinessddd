using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace WhatsAppBusiness.Infrastructure.WhatsApp;

/// <summary>
/// Validates WhatsApp webhook signatures using HMAC
/// </summary>
public class WhatsAppWebhookValidator
{
    private readonly ILogger<WhatsAppWebhookValidator> _logger;

    public WhatsAppWebhookValidator(ILogger<WhatsAppWebhookValidator> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates a webhook signature
    /// </summary>
    /// <param name="payload">Webhook payload</param>
    /// <param name="signature">Signature from X-Hub-Signature-256 header</param>
    /// <param name="appSecret">WhatsApp app secret</param>
    /// <returns>True if signature is valid, false otherwise</returns>
    public bool ValidateSignature(string payload, string signature, string appSecret)
    {
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException("Payload cannot be empty", nameof(payload));

        if (string.IsNullOrWhiteSpace(signature))
            throw new ArgumentException("Signature cannot be empty", nameof(signature));

        if (string.IsNullOrWhiteSpace(appSecret))
            throw new ArgumentException("App secret cannot be empty", nameof(appSecret));

        try
        {
            var expectedSignature = ComputeSignature(payload, appSecret);
            var isValid = signature.Equals(expectedSignature, StringComparison.OrdinalIgnoreCase);

            if (!isValid)
            {
                _logger.LogWarning("Webhook signature validation failed. Expected: {Expected}, Received: {Received}",
                    expectedSignature, signature);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating webhook signature");
            return false;
        }
    }

    /// <summary>
    /// Computes HMAC-SHA256 signature for webhook payload
    /// </summary>
    /// <param name="payload">Webhook payload</param>
    /// <param name="appSecret">WhatsApp app secret</param>
    /// <returns>Signature with "sha256=" prefix</returns>
    private static string ComputeSignature(string payload, string appSecret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(appSecret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(payloadBytes);
        var hashString = Convert.ToHexString(hashBytes).ToLowerInvariant();

        return $"sha256={hashString}";
    }
}
