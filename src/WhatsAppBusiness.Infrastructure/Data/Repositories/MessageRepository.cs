using Microsoft.EntityFrameworkCore;
using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.Interfaces.Repositories;

namespace WhatsAppBusiness.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Message entity
/// </summary>
public class MessageRepository : IMessageRepository
{
    private readonly WhatsAppBusinessDbContext _context;

    public MessageRepository(WhatsAppBusinessDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<Message?> GetByWhatsAppMessageIdAsync(string whatsAppMessageId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .FirstOrDefaultAsync(m => m.WhatsAppMessageId == whatsAppMessageId, cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetByConversationIdAsync(Guid conversationId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetLatestByConversationIdAsync(Guid conversationId, int count, CancellationToken cancellationToken = default)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return message;
    }

    public async Task UpdateAsync(Message message, CancellationToken cancellationToken = default)
    {
        _context.Messages.Update(message);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Message message, CancellationToken cancellationToken = default)
    {
        _context.Messages.Remove(message);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
