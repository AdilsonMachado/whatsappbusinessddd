using MediatR;
using WhatsAppBusiness.Application.Common;

namespace WhatsAppBusiness.Application.UseCases.Commands;

/// <summary>
/// Command to authenticate a user
/// </summary>
public class AuthenticateUserCommand : IRequest<Result>
{
    /// <summary>
    /// Gets or sets the user's phone number
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's name
    /// </summary>
    public string? Name { get; set; }
}
