using Microsoft.EntityFrameworkCore;
using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.Interfaces.Repositories;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Conversation entity
/// </summary>
public class ConversationRepository : IConversationRepository
{
    private readonly WhatsAppBusinessDbContext _context;

    public ConversationRepository(WhatsAppBusinessDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Conversation?> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Where(c => c.UserId == userId && c.Status != ConversationStatus.Closed)
            .OrderByDescending(c => c.UpdatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Conversation?> GetActiveByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Where(c => c.PhoneNumber == phoneNumber && c.Status != ConversationStatus.Closed)
            .OrderByDescending(c => c.UpdatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Conversation>> GetByOperatorIdAsync(Guid operatorId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Where(c => c.AssignedOperatorId == operatorId && c.Status != ConversationStatus.Closed)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Conversation>> GetByStatusAsync(ConversationStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Conversation> AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        await _context.Conversations.AddAsync(conversation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return conversation;
    }

    public async Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        _context.Conversations.Update(conversation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        _context.Conversations.Remove(conversation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
