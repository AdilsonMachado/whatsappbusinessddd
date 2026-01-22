using WhatsAppBusiness.Domain.Entities;

namespace WhatsAppBusiness.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for WhatsAppConfiguration entity
/// </summary>
public interface IConfigurationRepository
{
    /// <summary>
    /// Gets the active WhatsApp configuration
    /// </summary>
    Task<WhatsAppConfiguration?> GetActiveConfigurationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a configuration by ID
    /// </summary>
    Task<WhatsAppConfiguration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new configuration
    /// </summary>
    Task<WhatsAppConfiguration> AddAsync(WhatsAppConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing configuration
    /// </summary>
    Task UpdateAsync(WhatsAppConfiguration configuration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a configuration
    /// </summary>
    Task DeleteAsync(WhatsAppConfiguration configuration, CancellationToken cancellationToken = default);
}
