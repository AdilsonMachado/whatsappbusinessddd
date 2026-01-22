using WhatsAppBusiness.Domain.Entities;

namespace WhatsAppBusiness.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Operator entity
/// </summary>
public interface IOperatorRepository
{
    /// <summary>
    /// Gets an operator by ID
    /// </summary>
    Task<Operator?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an operator by email
    /// </summary>
    Task<Operator?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all online operators
    /// </summary>
    Task<IEnumerable<Operator>> GetOnlineOperatorsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the operator with the least number of active conversations
    /// </summary>
    Task<Operator?> GetLeastBusyOperatorAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new operator
    /// </summary>
    Task<Operator> AddAsync(Operator @operator, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing operator
    /// </summary>
    Task UpdateAsync(Operator @operator, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an operator
    /// </summary>
    Task DeleteAsync(Operator @operator, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an operator exists by email
    /// </summary>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}
