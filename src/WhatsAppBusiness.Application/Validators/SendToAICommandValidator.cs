using FluentValidation;
using WhatsAppBusiness.Application.UseCases.Commands;

namespace WhatsAppBusiness.Application.Validators;

/// <summary>
/// Validator for SendToAICommand
/// </summary>
public class SendToAICommandValidator : AbstractValidator<SendToAICommand>
{
    public SendToAICommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");
    }
}
