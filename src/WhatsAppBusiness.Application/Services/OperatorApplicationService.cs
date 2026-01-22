using AutoMapper;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Domain.Interfaces.Repositories;

namespace WhatsAppBusiness.Application.Services;

/// <summary>
/// Application service for operator operations
/// </summary>
public class OperatorApplicationService
{
    private readonly IOperatorRepository _operatorRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;

    public OperatorApplicationService(
        IOperatorRepository operatorRepository,
        IConversationRepository conversationRepository,
        IMapper mapper)
    {
        _operatorRepository = operatorRepository;
        _conversationRepository = conversationRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Gets an operator by ID
    /// </summary>
    public async Task<OperatorDto?> GetByIdAsync(Guid operatorId, CancellationToken cancellationToken = default)
    {
        var operator_ = await _operatorRepository.GetByIdAsync(operatorId, cancellationToken);
        return operator_ != null ? _mapper.Map<OperatorDto>(operator_) : null;
    }

    /// <summary>
    /// Gets an operator by email
    /// </summary>
    public async Task<OperatorDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var operator_ = await _operatorRepository.GetByEmailAsync(email, cancellationToken);
        return operator_ != null ? _mapper.Map<OperatorDto>(operator_) : null;
    }

    /// <summary>
    /// Gets all online operators
    /// </summary>
    public async Task<List<OperatorDto>> GetOnlineOperatorsAsync(CancellationToken cancellationToken = default)
    {
        var operators = await _operatorRepository.GetOnlineOperatorsAsync(cancellationToken);
        return _mapper.Map<List<OperatorDto>>(operators);
    }

    /// <summary>
    /// Sets operator online status
    /// </summary>
    public async Task SetOnlineStatusAsync(Guid operatorId, bool isOnline, CancellationToken cancellationToken = default)
    {
        var operator_ = await _operatorRepository.GetByIdAsync(operatorId, cancellationToken);
        if (operator_ == null)
            throw new InvalidOperationException("Operator not found");

        operator_.SetOnlineStatus(isOnline);
        await _operatorRepository.UpdateAsync(operator_, cancellationToken);
    }

    /// <summary>
    /// Gets the operator with the least conversations (for load balancing)
    /// </summary>
    public async Task<OperatorDto?> GetOperatorWithLeastConversationsAsync(CancellationToken cancellationToken = default)
    {
        var operators = await _operatorRepository.GetOnlineOperatorsAsync(cancellationToken);
        var operator_ = operators.OrderBy(o => o.CurrentConversations).FirstOrDefault();
        return operator_ != null ? _mapper.Map<OperatorDto>(operator_) : null;
    }
}
