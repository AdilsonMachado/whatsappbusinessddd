using FluentValidation;
using WhatsAppBusiness.Application.UseCases.Commands;

namespace WhatsAppBusiness.Application.Validators;

/// <summary>
/// Validator for AuthenticateUserCommand
/// </summary>
public class AuthenticateUserCommandValidator : AbstractValidator<AuthenticateUserCommand>
{
    public AuthenticateUserCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .SetValidator(new PhoneNumberValidator());

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Name cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}
