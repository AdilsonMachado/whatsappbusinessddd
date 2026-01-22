using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.Interfaces.Repositories;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Application.UseCases.Commands;

/// <summary>
/// Handler for AuthenticateUserCommand
/// </summary>
public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;

    public AuthenticateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var phoneNumber = PhoneNumber.Create(request.PhoneNumber);

            var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);

            if (user == null)
            {
                var name = request.Name ?? "User";
                user = User.Create(name, phoneNumber);
                await _userRepository.AddAsync(user, cancellationToken);
            }

            user.Authenticate();
            user.UpdateLastInteraction();

            if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != user.Name)
            {
                user.UpdateName(request.Name);
            }

            await _userRepository.UpdateAsync(user, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Authentication failed: {ex.Message}");
        }
    }
}
