using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entity
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by ID
    /// </summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by phone number
    /// </summary>
    Task<User?> GetByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user
    /// </summary>
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user
    /// </summary>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user
    /// </summary>
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists by phone number
    /// </summary>
    Task<bool> ExistsByPhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken = default);
}
