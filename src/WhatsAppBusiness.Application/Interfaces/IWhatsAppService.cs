using WhatsAppBusiness.Application.DTOs;

namespace WhatsAppBusiness.Application.Interfaces;

/// <summary>
/// Interface for WhatsApp messaging service
/// </summary>
public interface IWhatsAppService
{
    /// <summary>
    /// Sends a message via WhatsApp
    /// </summary>
    /// <param name="phoneNumber">Recipient's phone number</param>
    /// <param name="message">Message content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>WhatsApp message ID</returns>
    Task<string> SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes an incoming WhatsApp message
    /// </summary>
    /// <param name="whatsAppMessage">The WhatsApp message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ProcessIncomingMessageAsync(WhatsAppMessageDto whatsAppMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as delivered
    /// </summary>
    /// <param name="whatsAppMessageId">WhatsApp message ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task MarkAsDeliveredAsync(string whatsAppMessageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a message as read
    /// </summary>
    /// <param name="whatsAppMessageId">WhatsApp message ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task MarkAsReadAsync(string whatsAppMessageId, CancellationToken cancellationToken = default);
}
