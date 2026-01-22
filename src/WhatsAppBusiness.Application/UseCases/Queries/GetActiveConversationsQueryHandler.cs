using AutoMapper;
using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Domain.Interfaces.Repositories;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Application.UseCases.Queries;

/// <summary>
/// Handler for GetActiveConversationsQuery
/// </summary>
public class GetActiveConversationsQueryHandler : IRequestHandler<GetActiveConversationsQuery, Result<List<ConversationDto>>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly IMapper _mapper;

    public GetActiveConversationsQueryHandler(
        IConversationRepository conversationRepository,
        IUserRepository userRepository,
        IOperatorRepository operatorRepository,
        IMapper mapper)
    {
        _conversationRepository = conversationRepository;
        _userRepository = userRepository;
        _operatorRepository = operatorRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<ConversationDto>>> Handle(GetActiveConversationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var conversations = await _conversationRepository.GetByStatusAsync(ConversationStatus.WithAI, cancellationToken);
            var operatorConversations = await _conversationRepository.GetByStatusAsync(ConversationStatus.WithOperator, cancellationToken);
            
            var allConversations = conversations.Concat(operatorConversations).ToList();

            var conversationDtos = new List<ConversationDto>();

            foreach (var conversation in allConversations)
            {
                var dto = _mapper.Map<ConversationDto>(conversation);

                // Load user
                var user = await _userRepository.GetByIdAsync(conversation.UserId, cancellationToken);
                if (user != null)
                {
                    dto.User = _mapper.Map<UserDto>(user);
                }

                // Load operator if assigned
                if (conversation.AssignedOperatorId.HasValue)
                {
                    var operator_ = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
                    if (operator_ != null)
                    {
                        dto.AssignedOperator = _mapper.Map<OperatorDto>(operator_);
                    }
                }

                conversationDtos.Add(dto);
            }

            return Result.Success(conversationDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<ConversationDto>>($"Error retrieving active conversations: {ex.Message}");
        }
    }
}
