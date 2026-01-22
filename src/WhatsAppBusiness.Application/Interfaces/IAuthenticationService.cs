using WhatsAppBusiness.Application.DTOs;

namespace WhatsAppBusiness.Application.Interfaces;

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticates a user by phone number
    /// </summary>
    /// <param name="phoneNumber">User's phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User DTO if authenticated</returns>
    Task<UserDto?> AuthenticateUserAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates an operator by email and password
    /// </summary>
    /// <param name="email">Operator's email</param>
    /// <param name="password">Operator's password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operator DTO and token if authenticated</returns>
    Task<(OperatorDto? Operator, string? Token)> AuthenticateOperatorAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates an operator token
    /// </summary>
    /// <param name="token">Token to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operator DTO if token is valid</returns>
    Task<OperatorDto?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}
