using Microsoft.EntityFrameworkCore;
using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.Interfaces.Repositories;

namespace WhatsAppBusiness.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for Operator entity
/// </summary>
public class OperatorRepository : IOperatorRepository
{
    private readonly WhatsAppBusinessDbContext _context;

    public OperatorRepository(WhatsAppBusinessDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Operator?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Operators
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Operator?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Operators
            .FirstOrDefaultAsync(o => o.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<Operator>> GetOnlineOperatorsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Operators
            .Where(o => o.IsOnline)
            .ToListAsync(cancellationToken);
    }

    public async Task<Operator?> GetLeastBusyOperatorAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Operators
            .Where(o => o.IsOnline)
            .OrderBy(o => o.CurrentConversations)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Operator> AddAsync(Operator @operator, CancellationToken cancellationToken = default)
    {
        await _context.Operators.AddAsync(@operator, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return @operator;
    }

    public async Task UpdateAsync(Operator @operator, CancellationToken cancellationToken = default)
    {
        _context.Operators.Update(@operator);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Operator @operator, CancellationToken cancellationToken = default)
    {
        _context.Operators.Remove(@operator);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Operators
            .AnyAsync(o => o.Email == email, cancellationToken);
    }
}
