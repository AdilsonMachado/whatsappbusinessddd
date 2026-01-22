using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Application.Interfaces;
using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.Interfaces.Repositories;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Application.UseCases.Commands;

/// <summary>
/// Handler for SendMessageCommand
/// </summary>
public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IWhatsAppService _whatsAppService;

    public SendMessageCommandHandler(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        IWhatsAppService whatsAppService)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _whatsAppService = whatsAppService;
    }

    public async Task<Result> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
            if (conversation == null)
            {
                return Result.Failure("Conversation not found");
            }

            // Validate operator permission if message is from operator
            if (request.OperatorId.HasValue && conversation.AssignedOperatorId != request.OperatorId)
            {
                return Result.Failure("Operator not assigned to this conversation");
            }

            // Create message
            var messageContent = MessageContent.Create(request.Text);
            var message = Message.Create(
                conversation.Id,
                messageContent,
                MessageType.Text,
                MessageDirection.Outgoing,
                request.IsFromAI);

            await _messageRepository.AddAsync(message, cancellationToken);

            // Send via WhatsApp
            var whatsAppMessageId = await _whatsAppService.SendMessageAsync(
                conversation.PhoneNumber.Value,
                request.Text,
                cancellationToken);

            message.SetWhatsAppMessageId(whatsAppMessageId);

            // Update conversation timestamp
            conversation.UpdateTimestamp();

            await _messageRepository.UpdateAsync(message, cancellationToken);
            await _conversationRepository.UpdateAsync(conversation, cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to send message: {ex.Message}");
        }
    }
}
