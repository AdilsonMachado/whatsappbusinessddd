using WhatsAppBusiness.Domain.Entities;

namespace WhatsAppBusiness.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Message entity
/// </summary>
public interface IMessageRepository
{
    /// <summary>
    /// Gets a message by ID
    /// </summary>
    Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a message by WhatsApp message ID
    /// </summary>
    Task<Message?> GetByWhatsAppMessageIdAsync(string whatsAppMessageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all messages for a conversation
    /// </summary>
    Task<IEnumerable<Message>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets messages for a conversation with pagination
    /// </summary>
    Task<IEnumerable<Message>> GetByConversationIdAsync(Guid conversationId, int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the latest messages for a conversation
    /// </summary>
    Task<IEnumerable<Message>> GetLatestByConversationIdAsync(Guid conversationId, int count, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new message
    /// </summary>
    Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing message
    /// </summary>
    Task UpdateAsync(Message message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a message
    /// </summary>
    Task DeleteAsync(Message message, CancellationToken cancellationToken = default);
}
