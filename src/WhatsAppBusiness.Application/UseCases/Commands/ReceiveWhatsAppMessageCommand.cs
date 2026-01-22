using MediatR;
using WhatsAppBusiness.Application.Common;

namespace WhatsAppBusiness.Application.UseCases.Commands;

/// <summary>
/// Command to receive and process a WhatsApp message
/// </summary>
public class ReceiveWhatsAppMessageCommand : IRequest<Result>
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
    /// Gets or sets the message text
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
}
