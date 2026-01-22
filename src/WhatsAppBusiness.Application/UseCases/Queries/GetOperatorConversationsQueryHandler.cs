using AutoMapper;
using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Domain.Interfaces.Repositories;

namespace WhatsAppBusiness.Application.UseCases.Queries;

/// <summary>
/// Handler for GetOperatorConversationsQuery
/// </summary>
public class GetOperatorConversationsQueryHandler : IRequestHandler<GetOperatorConversationsQuery, Result<List<ConversationDto>>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly IMapper _mapper;

    public GetOperatorConversationsQueryHandler(
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

    public async Task<Result<List<ConversationDto>>> Handle(GetOperatorConversationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var operator_ = await _operatorRepository.GetByIdAsync(request.OperatorId, cancellationToken);
            if (operator_ == null)
            {
                return Result.Failure<List<ConversationDto>>("Operator not found");
            }

            var conversations = await _conversationRepository.GetByOperatorIdAsync(request.OperatorId, cancellationToken);

            var conversationDtos = new List<ConversationDto>();

            foreach (var conversation in conversations)
            {
                var dto = _mapper.Map<ConversationDto>(conversation);

                // Load user
                var user = await _userRepository.GetByIdAsync(conversation.UserId, cancellationToken);
                if (user != null)
                {
                    dto.User = _mapper.Map<UserDto>(user);
                }

                // Set operator
                dto.AssignedOperator = _mapper.Map<OperatorDto>(operator_);

                conversationDtos.Add(dto);
            }

            return Result.Success(conversationDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<ConversationDto>>($"Error retrieving operator conversations: {ex.Message}");
        }
    }
}
