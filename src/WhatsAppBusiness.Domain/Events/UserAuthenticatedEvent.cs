using WhatsAppBusiness.Domain.Common;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Domain.Events;

/// <summary>
/// Event raised when a user is successfully authenticated
/// </summary>
public class UserAuthenticatedEvent : DomainEvent
{
    /// <summary>
    /// Gets the user ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Gets the phone number
    /// </summary>
    public PhoneNumber PhoneNumber { get; }

    /// <summary>
    /// Gets the user name
    /// </summary>
    public string UserName { get; }

    /// <summary>
    /// Gets the conversation ID
    /// </summary>
    public Guid ConversationId { get; }

    /// <summary>
    /// Creates a new UserAuthenticatedEvent
    /// </summary>
    public UserAuthenticatedEvent(Guid userId, PhoneNumber phoneNumber, string userName, Guid conversationId)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
        UserName = userName;
        ConversationId = conversationId;
    }
}
