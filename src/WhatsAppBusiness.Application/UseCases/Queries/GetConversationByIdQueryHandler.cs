using AutoMapper;
using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Domain.Interfaces.Repositories;

namespace WhatsAppBusiness.Application.UseCases.Queries;

/// <summary>
/// Handler for GetConversationByIdQuery
/// </summary>
public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, Result<ConversationDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public GetConversationByIdQueryHandler(
        IConversationRepository conversationRepository,
        IUserRepository userRepository,
        IOperatorRepository operatorRepository,
        IMessageRepository messageRepository,
        IMapper mapper)
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
        _operatorRepository = operatorRepository;
        _messageRepository = messageRepository;
        _mapper = mapper;
    }

    public async Task<Result<ConversationDto>> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
            if (conversation == null)
            {
                return Result.Failure<ConversationDto>("Conversation not found");
            }

            var conversationDto = _mapper.Map<ConversationDto>(conversation);

            // Load user
            var user = await _userRepository.GetByIdAsync(conversation.UserId, cancellationToken);
            if (user != null)
            {
                conversationDto.User = _mapper.Map<UserDto>(user);
            }

            // Load operator if assigned
            if (conversation.AssignedOperatorId.HasValue)
            {
                var operator_ = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
                if (operator_ != null)
                {
                    conversationDto.AssignedOperator = _mapper.Map<OperatorDto>(operator_);
                }
            }

            // Load messages if requested
            if (request.IncludeMessages)
            {
                var messages = await _messageRepository.GetByConversationIdAsync(conversation.Id, cancellationToken);
                conversationDto.Messages = _mapper.Map<List<MessageDto>>(messages);
            }

            return Result.Success(conversationDto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ConversationDto>($"Error retrieving conversation: {ex.Message}");
        }
    }
}
