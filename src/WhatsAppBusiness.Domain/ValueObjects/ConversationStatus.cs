namespace WhatsAppBusiness.Domain.ValueObjects;

/// <summary>
/// Represents the status of a conversation
/// </summary>
public enum ConversationStatus
{
    /// <summary>
    /// User is waiting to be authenticated
    /// </summary>
    WaitingAuthentication,

    /// <summary>
    /// Conversation is being handled by AI
    /// </summary>
    WithAI,

    /// <summary>
    /// Conversation has been assigned to a human operator
    /// </summary>
    WithOperator,

    /// <summary>
    /// Conversation has been closed
    /// </summary>
    Closed
}
