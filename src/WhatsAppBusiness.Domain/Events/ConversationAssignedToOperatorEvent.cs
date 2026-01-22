using WhatsAppBusiness.Domain.Common;

namespace WhatsAppBusiness.Domain.Events;

/// <summary>
/// Event raised when a conversation is assigned to an operator
/// </summary>
public class ConversationAssignedToOperatorEvent : DomainEvent
{
    /// <summary>
    /// Gets the conversation ID
    /// </summary>
    public Guid ConversationId { get; }

    /// <summary>
    /// Gets the operator ID
    /// </summary>
    public Guid OperatorId { get; }

    /// <summary>
    /// Gets the operator name
    /// </summary>
    public string OperatorName { get; }

    /// <summary>
    /// Gets the user ID
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Gets the previous status of the conversation
    /// </summary>
    public string PreviousStatus { get; }

    /// <summary>
    /// Creates a new ConversationAssignedToOperatorEvent
    /// </summary>
    public ConversationAssignedToOperatorEvent(
        Guid conversationId,
        Guid operatorId,
        string operatorName,
        Guid userId,
        string previousStatus)
    {
        ConversationId = conversationId;
        OperatorId = operatorId;
        OperatorName = operatorName;
        UserId = userId;
        PreviousStatus = previousStatus;
    }
}
