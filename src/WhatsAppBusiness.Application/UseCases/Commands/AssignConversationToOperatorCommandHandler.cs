using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Domain.Interfaces.Repositories;

namespace WhatsAppBusiness.Application.UseCases.Commands;

/// <summary>
/// Handler for AssignConversationToOperatorCommand
/// </summary>
public class AssignConversationToOperatorCommandHandler : IRequestHandler<AssignConversationToOperatorCommand, Result>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IOperatorRepository _operatorRepository;

    public AssignConversationToOperatorCommandHandler(
        IConversationRepository conversationRepository,
        IOperatorRepository operatorRepository)
    {
        _conversationRepository = conversationRepository;
        _operatorRepository = operatorRepository;
    }

    public async Task<Result> Handle(AssignConversationToOperatorCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
            if (conversation == null)
            {
                return Result.Failure("Conversation not found");
            }

            var operator_ = await _operatorRepository.GetByIdAsync(request.OperatorId, cancellationToken);
            if (operator_ == null)
            {
                return Result.Failure("Operator not found");
            }

            // If conversation already assigned to another operator, unassign from that operator
            if (conversation.AssignedOperatorId.HasValue && conversation.AssignedOperatorId != request.OperatorId)
            {
                var previousOperator = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
                if (previousOperator != null)
                {
                    previousOperator.UnassignConversation();
                    await _operatorRepository.UpdateAsync(previousOperator, cancellationToken);
                }
            }

            // Assign to new operator
            conversation.AssignToOperator(request.OperatorId);
            operator_.AssignConversation();

            await _conversationRepository.UpdateAsync(conversation, cancellationToken);
            await _operatorRepository.UpdateAsync(operator_, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to assign conversation: {ex.Message}");
        }
    }
}
