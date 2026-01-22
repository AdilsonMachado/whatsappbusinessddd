using WhatsAppBusiness.Application.DTOs;

namespace WhatsAppBusiness.Application.Interfaces;

/// <summary>
/// Interface for conversation-related application services
/// </summary>
public interface IConversationService
{
    /// <summary>
    /// Gets a conversation by ID
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Conversation DTO</returns>
    Task<ConversationDto?> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active conversations
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active conversations</returns>
    Task<List<ConversationDto>> GetActiveConversationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets conversations assigned to a specific operator
    /// </summary>
    /// <param name="operatorId">Operator ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of conversations assigned to the operator</returns>
    Task<List<ConversationDto>> GetOperatorConversationsAsync(Guid operatorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a conversation to an operator
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="operatorId">Operator ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AssignToOperatorAsync(Guid conversationId, Guid operatorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a conversation to AI
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendToAIAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes a conversation
    /// </summary>
    /// <param name="conversationId">Conversation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CloseConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);
}
