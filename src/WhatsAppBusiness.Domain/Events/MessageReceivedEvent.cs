using WhatsAppBusiness.Domain.Common;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Domain.Events;

/// <summary>
/// Event raised when a message is received from a user
/// </summary>
public class MessageReceivedEvent : DomainEvent
{
    /// <summary>
    /// Gets the conversation ID
    /// </summary>
    public Guid ConversationId { get; }

    /// <summary>
    /// Gets the message ID
    /// </summary>
    public Guid MessageId { get; }

    /// <summary>
    /// Gets the user ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Gets the phone number
    /// </summary>
    public PhoneNumber PhoneNumber { get; }

    /// <summary>
    /// Gets the message content
    /// </summary>
    public MessageContent Content { get; }

    /// <summary>
    /// Gets the message type
    /// </summary>
    public MessageType Type { get; }

    /// <summary>
    /// Creates a new MessageReceivedEvent
    /// </summary>
    public MessageReceivedEvent(
        Guid conversationId,
        Guid messageId,
        Guid userId,
        PhoneNumber phoneNumber,
        MessageContent content,
        MessageType type)
    {
        ConversationId = conversationId;
        MessageId = messageId;
        UserId = userId;
        PhoneNumber = phoneNumber;
        Content = content;
        Type = type;
    }
}
