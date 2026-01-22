namespace WhatsAppBusiness.Application.DTOs;

/// <summary>
/// Data transfer object for WhatsApp message from API
/// </summary>
public class WhatsAppMessageDto
{
    /// <summary>
    /// Gets or sets the WhatsApp message ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the sender's phone number
    /// </summary>
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the recipient's phone number
    /// </summary>
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message text content
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message type
    /// </summary>
    public string Type { get; set; } = "text";

    /// <summary>
    /// Gets or sets the timestamp
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the message status
    /// </summary>
    public string? Status { get; set; }
}
