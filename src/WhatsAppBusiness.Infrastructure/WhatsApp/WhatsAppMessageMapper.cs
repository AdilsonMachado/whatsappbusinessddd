using System.Text.Json;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Infrastructure.WhatsApp;

/// <summary>
/// Maps between WhatsApp API format and domain models
/// </summary>
public class WhatsAppMessageMapper
{
    /// <summary>
    /// Extracts message details from WhatsApp webhook payload
    /// </summary>
    /// <param name="webhookPayload">Webhook JSON payload</param>
    /// <returns>Tuple of (phoneNumber, messageContent, messageId)</returns>
    public (string phoneNumber, string messageContent, string messageId) ExtractIncomingMessage(string webhookPayload)
    {
        if (string.IsNullOrWhiteSpace(webhookPayload))
            throw new ArgumentException("Webhook payload cannot be empty", nameof(webhookPayload));

        using var document = JsonDocument.Parse(webhookPayload);
        var root = document.RootElement;

        var entry = root.GetProperty("entry")[0];
        var changes = entry.GetProperty("changes")[0];
        var value = changes.GetProperty("value");
        var messages = value.GetProperty("messages")[0];

        var phoneNumber = messages.GetProperty("from").GetString()
            ?? throw new InvalidOperationException("Phone number not found in webhook payload");

        var messageId = messages.GetProperty("id").GetString()
            ?? throw new InvalidOperationException("Message ID not found in webhook payload");

        var messageType = messages.GetProperty("type").GetString();
        var messageContent = messageType switch
        {
            "text" => messages.GetProperty("text").GetProperty("body").GetString(),
            "image" => "📷 Image received",
            "audio" => "🎵 Audio received",
            "video" => "🎥 Video received",
            "document" => "📄 Document received",
            "location" => "📍 Location received",
            "contacts" => "👤 Contact received",
            "sticker" => "🎨 Sticker received",
            _ => "Unsupported message type"
        } ?? "Empty message";

        return (phoneNumber, messageContent, messageId);
    }

    /// <summary>
    /// Determines message type from WhatsApp API type
    /// </summary>
    /// <param name="whatsAppType">WhatsApp message type string</param>
    /// <returns>Domain MessageType</returns>
    public MessageType MapMessageType(string whatsAppType)
    {
        return whatsAppType?.ToLowerInvariant() switch
        {
            "text" => MessageType.Text,
            "image" => MessageType.Image,
            "audio" => MessageType.Audio,
            "video" => MessageType.Video,
            "document" => MessageType.Document,
            "location" => MessageType.Location,
            "contacts" => MessageType.Contact,
            "sticker" => MessageType.Sticker,
            _ => MessageType.Text
        };
    }

    /// <summary>
    /// Checks if webhook payload contains a message
    /// </summary>
    /// <param name="webhookPayload">Webhook JSON payload</param>
    /// <returns>True if payload contains a message</returns>
    public bool IsMessageWebhook(string webhookPayload)
    {
        if (string.IsNullOrWhiteSpace(webhookPayload))
            return false;

        try
        {
            using var document = JsonDocument.Parse(webhookPayload);
            var root = document.RootElement;

            if (!root.TryGetProperty("entry", out var entry))
                return false;

            if (entry.GetArrayLength() == 0)
                return false;

            var changes = entry[0].GetProperty("changes")[0];
            var value = changes.GetProperty("value");

            return value.TryGetProperty("messages", out var messages) && messages.GetArrayLength() > 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if webhook payload contains a status update
    /// </summary>
    /// <param name="webhookPayload">Webhook JSON payload</param>
    /// <returns>True if payload contains a status update</returns>
    public bool IsStatusWebhook(string webhookPayload)
    {
        if (string.IsNullOrWhiteSpace(webhookPayload))
            return false;

        try
        {
            using var document = JsonDocument.Parse(webhookPayload);
            var root = document.RootElement;

            if (!root.TryGetProperty("entry", out var entry))
                return false;

            if (entry.GetArrayLength() == 0)
                return false;

            var changes = entry[0].GetProperty("changes")[0];
            var value = changes.GetProperty("value");

            return value.TryGetProperty("statuses", out var statuses) && statuses.GetArrayLength() > 0;
        }
        catch
        {
            return false;
        }
    }
}
