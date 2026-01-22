using FluentValidation;
using WhatsAppBusiness.Application.UseCases.Commands;

namespace WhatsAppBusiness.Application.Validators;

/// <summary>
/// Validator for AssignConversationToOperatorCommand
/// </summary>
public class AssignConversationToOperatorCommandValidator : AbstractValidator<AssignConversationToOperatorCommand>
{
    public AssignConversationToOperatorCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .WithMessage("Conversation ID is required");

        RuleFor(x => x.OperatorId)
            .NotEmpty()
            .WithMessage("Operator ID is required");
    }
}
