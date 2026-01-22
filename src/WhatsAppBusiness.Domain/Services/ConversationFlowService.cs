using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.Interfaces.Repositories;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Domain.Services;

/// <summary>
/// Domain service for conversation flow business rules
/// </summary>
public class ConversationFlowService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IOperatorRepository _operatorRepository;

    public ConversationFlowService(
        IConversationRepository conversationRepository,
        IOperatorRepository operatorRepository)
    {
        _conversationRepository = conversationRepository ?? throw new ArgumentNullException(nameof(conversationRepository));
        _operatorRepository = operatorRepository ?? throw new ArgumentNullException(nameof(operatorRepository));
    }

    /// <summary>
    /// Gets or creates an active conversation for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="phoneNumber">The user's phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The active conversation</returns>
    public async Task<Conversation> GetOrCreateActiveConversationAsync(
        Guid userId,
        PhoneNumber phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetActiveByUserIdAsync(userId, cancellationToken);

        if (conversation == null)
        {
            conversation = Conversation.Create(userId, phoneNumber);
            await _conversationRepository.AddAsync(conversation, cancellationToken);
        }
        else
        {
            conversation.UpdateTimestamp();
            await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        }

        return conversation;
    }

    /// <summary>
    /// Assigns a conversation to an available operator
    /// </summary>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The assigned operator, or null if no operators are available</returns>
    public async Task<Operator?> AssignConversationToOperatorAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            throw new InvalidOperationException($"Conversation with ID {conversationId} not found");

        var availableOperator = await _operatorRepository.GetLeastBusyOperatorAsync(cancellationToken);
        if (availableOperator == null)
            return null;

        conversation.AssignToOperator(availableOperator.Id);
        availableOperator.AssignConversation();

        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
        await _operatorRepository.UpdateAsync(availableOperator, cancellationToken);

        return availableOperator;
    }

    /// <summary>
    /// Transfers a conversation from AI to an operator
    /// </summary>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The assigned operator, or null if no operators are available</returns>
    public async Task<Operator?> TransferToOperatorAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            throw new InvalidOperationException($"Conversation with ID {conversationId} not found");

        if (conversation.Status == ConversationStatus.WithOperator)
            throw new InvalidOperationException("Conversation is already assigned to an operator");

        return await AssignConversationToOperatorAsync(conversationId, cancellationToken);
    }

    /// <summary>
    /// Closes a conversation and updates operator assignment
    /// </summary>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task CloseConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            throw new InvalidOperationException($"Conversation with ID {conversationId} not found");

        if (conversation.Status == ConversationStatus.Closed)
            return;

        if (conversation.AssignedOperatorId.HasValue)
        {
            var @operator = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
            if (@operator != null)
            {
                @operator.UnassignConversation();
                await _operatorRepository.UpdateAsync(@operator, cancellationToken);
            }
        }

        conversation.Close();
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
    }

    /// <summary>
    /// Determines if a conversation should be escalated to a human operator
    /// </summary>
    /// <param name="conversation">The conversation</param>
    /// <returns>True if escalation is needed</returns>
    public bool ShouldEscalateToOperator(Conversation conversation)
    {
        if (conversation == null)
            throw new ArgumentNullException(nameof(conversation));

        return conversation.Status == ConversationStatus.WithAI && conversation.IsWithAI;
    }

    /// <summary>
    /// Starts AI handling for a conversation
    /// </summary>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task StartAIHandlingAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            throw new InvalidOperationException($"Conversation with ID {conversationId} not found");

        if (conversation.AssignedOperatorId.HasValue)
        {
            var @operator = await _operatorRepository.GetByIdAsync(conversation.AssignedOperatorId.Value, cancellationToken);
            if (@operator != null)
            {
                @operator.UnassignConversation();
                await _operatorRepository.UpdateAsync(@operator, cancellationToken);
            }
        }

        conversation.StartAIHandling();
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);
    }
}
