using FluentValidation;
using WhatsAppBusiness.Application.UseCases.Commands;

namespace WhatsAppBusiness.Application.Validators;

/// <summary>
/// Validator for ReceiveWhatsAppMessageCommand
/// </summary>
public class ReceiveWhatsAppMessageCommandValidator : AbstractValidator<ReceiveWhatsAppMessageCommand>
{
    public ReceiveWhatsAppMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty()
            .WithMessage("Message ID is required");

        RuleFor(x => x.From)
            .NotEmpty()
            .WithMessage("Sender phone number is required")
            .SetValidator(new PhoneNumberValidator());

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Message text is required")
            .SetValidator(new MessageContentValidator());

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("Message type is required");

        RuleFor(x => x.Timestamp)
            .NotEmpty()
            .WithMessage("Timestamp is required")
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
            .WithMessage("Timestamp cannot be in the future");
    }
}
