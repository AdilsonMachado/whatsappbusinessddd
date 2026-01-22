using FluentValidation;

namespace WhatsAppBusiness.Application.Validators;

/// <summary>
/// Validator for message content
/// </summary>
public class MessageContentValidator : AbstractValidator<string>
{
    private const int MaxLength = 4096;

    public MessageContentValidator()
    {
        RuleFor(content => content)
            .NotEmpty()
            .WithMessage("Message content cannot be empty")
            .MaximumLength(MaxLength)
            .WithMessage($"Message content cannot exceed {MaxLength} characters");
    }
}
