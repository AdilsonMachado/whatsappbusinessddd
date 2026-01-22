using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Conversation entity
/// </summary>
public interface IConversationRepository
{
    /// <summary>
    /// Gets a conversation by ID
    /// </summary>
    Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active conversation by user ID
    /// </summary>
    Task<Conversation?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active conversation by phone number
    /// </summary>
    Task<Conversation?> GetActiveByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all conversations assigned to an operator
    /// </summary>
    Task<IEnumerable<Conversation>> GetByOperatorIdAsync(Guid operatorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all conversations with a specific status
    /// </summary>
    Task<IEnumerable<Conversation>> GetByStatusAsync(ConversationStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all conversations for a user
    /// </summary>
    Task<IEnumerable<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new conversation
    /// </summary>
    Task<Conversation> AddAsync(Conversation conversation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing conversation
    /// </summary>
    Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a conversation
    /// </summary>
    Task DeleteAsync(Conversation conversation, CancellationToken cancellationToken = default);
}
