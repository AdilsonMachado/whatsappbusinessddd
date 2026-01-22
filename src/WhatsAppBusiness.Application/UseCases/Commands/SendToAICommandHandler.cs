using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Domain.Interfaces.Repositories;

namespace WhatsAppBusiness.Application.UseCases.Commands;

/// <summary>
/// Handler for SendToAICommand
/// </summary>
public class SendToAICommandHandler : IRequestHandler<SendToAICommand, Result>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IOperatorRepository _operatorRepository;

    public SendToAICommandHandler(
        IConversationRepository conversationRepository,
        IOperatorRepository operatorRepository)
    {
        _conversationRepository = conversationRepository;
        _operatorRepository = operatorRepository;
    }

    public async Task<Result> Handle(SendToAICommand request, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
            if (conversation == null)
            {
                return Result.Failure("Conversation not found");
            }

            // If conversation was with operator, unassign
            if (conversation.AssignedOperatorId.HasValue)
            {
                var operator_ = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
                if (operator_ != null)
                {
                    operator_.UnassignConversation();
                    await _operatorRepository.UpdateAsync(operator_, cancellationToken);
                }
            }

            // Send to AI
            conversation.StartAIHandling();

            await _conversationRepository.UpdateAsync(conversation, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to send to AI: {ex.Message}");
        }
    }
}
