using Microsoft.EntityFrameworkCore;
using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.Interfaces.Repositories;

namespace WhatsAppBusiness.Infrastructure.Data.Repositories;

/// <summary>
/// Repository implementation for WhatsAppConfiguration entity
/// </summary>
public class ConfigurationRepository : IConfigurationRepository
{
    private readonly WhatsAppBusinessDbContext _context;

    public ConfigurationRepository(WhatsAppBusinessDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<WhatsAppConfiguration?> GetActiveConfigurationAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WhatsAppConfigurations
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<WhatsAppConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WhatsAppConfigurations
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<WhatsAppConfiguration> AddAsync(WhatsAppConfiguration configuration, CancellationToken cancellationToken = default)
    {
        await _context.WhatsAppConfigurations.AddAsync(configuration, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return configuration;
    }

    public async Task UpdateAsync(WhatsAppConfiguration configuration, CancellationToken cancellationToken = default)
    {
        _context.WhatsAppConfigurations.Update(configuration);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(WhatsAppConfiguration configuration, CancellationToken cancellationToken = default)
    {
        _context.WhatsAppConfigurations.Remove(configuration);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
