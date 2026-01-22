using FluentValidation;
using WhatsAppBusiness.Application.UseCases.Commands;

namespace WhatsAppBusiness.Application.Validators;

/// <summary>
/// Validator for SendMessageCommand
/// </summary>
public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Message text is required")
            .SetValidator(new MessageContentValidator());
    }
}
