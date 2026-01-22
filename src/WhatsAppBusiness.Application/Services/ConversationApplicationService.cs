using AutoMapper;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Application.Interfaces;
using WhatsAppBusiness.Domain.Interfaces.Repositories;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Application.Services;

/// <summary>
/// Application service for conversation orchestration
/// </summary>
public class ConversationApplicationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;

    public ConversationApplicationService(
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

    public async Task<ConversationDto?> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            return null;

        var dto = _mapper.Map<ConversationDto>(conversation);

        // Load related entities
        var user = await _userRepository.GetByIdAsync(conversation.UserId, cancellationToken);
        if (user != null)
        {
            dto.User = _mapper.Map<UserDto>(user);
        }

        if (conversation.AssignedOperatorId.HasValue)
        {
            var operator_ = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
            if (operator_ != null)
            {
                dto.AssignedOperator = _mapper.Map<OperatorDto>(operator_);
            }
        }

        var messages = await _messageRepository.GetByConversationIdAsync(conversation.Id, cancellationToken);
        dto.Messages = _mapper.Map<List<MessageDto>>(messages.OrderBy(m => m.SentAt).ToList());

        return dto;
    }

    public async Task<List<ConversationDto>> GetActiveConversationsAsync(CancellationToken cancellationToken = default)
    {
        var withAI = await _conversationRepository.GetByStatusAsync(ConversationStatus.WithAI, cancellationToken);
        var withOperator = await _conversationRepository.GetByStatusAsync(ConversationStatus.WithOperator, cancellationToken);

        var allConversations = withAI.Concat(withOperator).ToList();
        var dtos = new List<ConversationDto>();

        foreach (var conversation in allConversations)
        {
            var dto = _mapper.Map<ConversationDto>(conversation);

            var user = await _userRepository.GetByIdAsync(conversation.UserId, cancellationToken);
            if (user != null)
            {
                dto.User = _mapper.Map<UserDto>(user);
            }

            if (conversation.AssignedOperatorId.HasValue)
            {
                var operator_ = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
                if (operator_ != null)
                {
                    dto.AssignedOperator = _mapper.Map<OperatorDto>(operator_);
                }
            }

            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<List<ConversationDto>> GetOperatorConversationsAsync(Guid operatorId, CancellationToken cancellationToken = default)
    {
        var conversations = await _conversationRepository.GetByOperatorIdAsync(operatorId, cancellationToken);
        var dtos = new List<ConversationDto>();

        var operator_ = await _operatorRepository.GetByIdAsync(operatorId, cancellationToken);

        foreach (var conversation in conversations)
        {
            var dto = _mapper.Map<ConversationDto>(conversation);

            var user = await _userRepository.GetByIdAsync(conversation.UserId, cancellationToken);
            if (user != null)
            {
                dto.User = _mapper.Map<UserDto>(user);
            }

            if (operator_ != null)
            {
                dto.AssignedOperator = _mapper.Map<OperatorDto>(operator_);
            }

            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task AssignToOperatorAsync(Guid conversationId, Guid operatorId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            throw new InvalidOperationException("Conversation not found");

        var operator_ = await _operatorRepository.GetByIdAsync(operatorId, cancellationToken);
        if (operator_ == null)
            throw new InvalidOperationException("Operator not found");

        // Unassign from previous operator if needed
        if (conversation.AssignedOperatorId.HasValue && conversation.AssignedOperatorId != operatorId)
        {
            var previousOperator = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
            if (previousOperator != null)
            {
                previousOperator.UnassignConversation();
                await _operatorRepository.UpdateAsync(previousOperator, cancellationToken);
            }
        }

        conversation.AssignToOperator(operatorId);
        operator_.AssignConversation();

        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        await _operatorRepository.UpdateAsync(operator_, cancellationToken);
    }

    public async Task SendToAIAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            throw new InvalidOperationException("Conversation not found");

        // Unassign from operator if needed
        if (conversation.AssignedOperatorId.HasValue)
        {
            var operator_ = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
            if (operator_ != null)
            {
                operator_.UnassignConversation();
                await _operatorRepository.UpdateAsync(operator_, cancellationToken);
            }
        }

        conversation.StartAIHandling();
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
    }

    public async Task CloseConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            throw new InvalidOperationException("Conversation not found");

        // Unassign from operator if needed
        if (conversation.AssignedOperatorId.HasValue)
        {
            var operator_ = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
            if (operator_ != null)
            {
                operator_.UnassignConversation();
                await _operatorRepository.UpdateAsync(operator_, cancellationToken);
            }
        }

        conversation.Close();
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
    }
}
