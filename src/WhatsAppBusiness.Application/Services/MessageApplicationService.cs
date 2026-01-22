using AutoMapper;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Application.Interfaces;
using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.Interfaces.Repositories;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Application.Services;

/// <summary>
/// Application service for message operations
/// </summary>
public class MessageApplicationService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IWhatsAppService _whatsAppService;
    private readonly IMapper _mapper;

    public MessageApplicationService(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository,
        IWhatsAppService whatsAppService,
        IMapper mapper)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _whatsAppService = whatsAppService;
        _mapper = mapper;
    }

    /// <summary>
    /// Sends a message in a conversation
    /// </summary>
    public async Task<MessageDto> SendMessageAsync(
        Guid conversationId,
        string text,
        bool isFromAI = false,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            throw new InvalidOperationException("Conversation not found");

        var messageContent = MessageContent.Create(text);
        var message = Message.Create(
            conversationId,
            messageContent,
            MessageType.Text,
            MessageDirection.Outgoing,
            isFromAI);

        await _messageRepository.AddAsync(message, cancellationToken);

        // Send via WhatsApp
        var whatsAppMessageId = await _whatsAppService.SendMessageAsync(
            conversation.PhoneNumber.Value,
            text,
            cancellationToken);

        message.SetWhatsAppMessageId(whatsAppMessageId);

        // Update conversation timestamp
        conversation.UpdateTimestamp();

        await _messageRepository.UpdateAsync(message, cancellationToken);
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);

        return _mapper.Map<MessageDto>(message);
    }

    /// <summary>
    /// Gets messages for a conversation
    /// </summary>
    public async Task<List<MessageDto>> GetConversationMessagesAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var messages = await _messageRepository.GetByConversationIdAsync(conversationId, cancellationToken);
        return _mapper.Map<List<MessageDto>>(messages.OrderBy(m => m.SentAt).ToList());
    }

    /// <summary>
    /// Marks a message as delivered
    /// </summary>
    public async Task MarkAsDeliveredAsync(string whatsAppMessageId, CancellationToken cancellationToken = default)
    {
        var message = await _messageRepository.GetByWhatsAppMessageIdAsync(whatsAppMessageId, cancellationToken);
        if (message != null)
        {
            message.MarkAsDelivered();
            await _messageRepository.UpdateAsync(message, cancellationToken);
        }
    }

    /// <summary>
    /// Marks a message as read
    /// </summary>
    public async Task MarkAsReadAsync(string whatsAppMessageId, CancellationToken cancellationToken = default)
    {
        var message = await _messageRepository.GetByWhatsAppMessageIdAsync(whatsAppMessageId, cancellationToken);
        if (message != null)
        {
            message.MarkAsRead();
            await _messageRepository.UpdateAsync(message, cancellationToken);
        }
    }
}
