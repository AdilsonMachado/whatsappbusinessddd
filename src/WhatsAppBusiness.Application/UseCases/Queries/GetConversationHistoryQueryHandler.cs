using AutoMapper;
using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Domain.Interfaces.Repositories;

namespace WhatsAppBusiness.Application.UseCases.Queries;

/// <summary>
/// Handler for GetConversationHistoryQuery
/// </summary>
public class GetConversationHistoryQueryHandler : IRequestHandler<GetConversationHistoryQuery, Result<List<MessageDto>>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetConversationHistoryQueryHandler(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<MessageDto>>> Handle(GetConversationHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
            if (conversation == null)
            {
                return Result.Failure<List<MessageDto>>("Conversation not found");
            }

            var messages = await _messageRepository.GetByConversationIdAsync(conversation.Id, cancellationToken);

            // Sort by sent date ascending (oldest first)
            messages = messages.OrderBy(m => m.SentAt).ToList();

            // Apply limit if specified
            if (request.Limit.HasValue && request.Limit.Value > 0)
            {
                messages = messages.TakeLast(request.Limit.Value).ToList();
            }

            var messageDtos = _mapper.Map<List<MessageDto>>(messages);

            return Result.Success(messageDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<MessageDto>>($"Error retrieving conversation history: {ex.Message}");
        }
    }
}
