using AutoMapper;
using MediatR;
using WhatsAppBusiness.Application.Common;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Application.Interfaces;
using WhatsAppBusiness.Domain.Entities;
using WhatsAppBusiness.Domain.Interfaces.Repositories;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.Application.UseCases.Commands;

/// <summary>
/// Handler for ReceiveWhatsAppMessageCommand
/// </summary>
public class ReceiveWhatsAppMessageCommandHandler : IRequestHandler<ReceiveWhatsAppMessageCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IMCPService _mcpService;
    private readonly IWhatsAppService _whatsAppService;
    private readonly IMapper _mapper;

    public ReceiveWhatsAppMessageCommandHandler(
        IUserRepository userRepository,
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        IMCPService mcpService,
        IWhatsAppService whatsAppService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _mcpService = mcpService;
        _whatsAppService = whatsAppService;
        _mapper = mapper;
    }

    public async Task<Result> Handle(ReceiveWhatsAppMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Normalize phone number
            var phoneNumber = PhoneNumber.Create(request.From);

            // Find or create user
            var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber, cancellationToken);
            if (user == null)
            {
                user = User.Create("User", phoneNumber);
                await _userRepository.AddAsync(user, cancellationToken);
            }

            user.UpdateLastInteraction();

            // Find or create active conversation
            var conversation = await _conversationRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);
            if (conversation == null)
            {
                conversation = Conversation.Create(user.Id, phoneNumber);
                await _conversationRepository.AddAsync(conversation, cancellationToken);
            }

            // Create incoming message
            var messageContent = MessageContent.Create(request.Text);
            var message = Message.Create(
                conversation.Id,
                messageContent,
                MessageType.Text,
                MessageDirection.Incoming,
                isFromAI: false);

            message.SetWhatsAppMessageId(request.MessageId);
            await _messageRepository.AddAsync(message, cancellationToken);

            conversation.UpdateTimestamp();

            // Save changes
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _conversationRepository.UpdateAsync(conversation, cancellationToken);

            // Process based on conversation status
            if (conversation.Status == ConversationStatus.WaitingAuthentication)
            {
                // Authenticate user
                user.Authenticate();
                conversation.MarkAsAuthenticated();
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _conversationRepository.UpdateAsync(conversation, cancellationToken);

                // Send welcome message
                var welcomeText = $"Hello {user.Name}! Welcome to our WhatsApp Business service. How can I help you today?";
                await SendOutgoingMessageAsync(conversation.Id, welcomeText, true, cancellationToken);
            }
            else if (conversation.Status == ConversationStatus.WithAI)
            {
                // Get conversation history
                var history = await _messageRepository.GetByConversationIdAsync(conversation.Id, cancellationToken);
                var historyDtos = _mapper.Map<List<MessageDto>>(history);

                // Send to AI
                var mcpRequest = new MCPRequestDto
                {
                    ConversationId = conversation.Id,
                    Message = request.Text,
                    History = historyDtos,
                    Context = new Dictionary<string, string>
                    {
                        ["userId"] = user.Id.ToString(),
                        ["userName"] = user.Name,
                        ["phoneNumber"] = phoneNumber.Value
                    }
                };

                var mcpResponse = await _mcpService.SendRequestAsync(mcpRequest, cancellationToken);

                if (mcpResponse.IsSuccess)
                {
                    await SendOutgoingMessageAsync(conversation.Id, mcpResponse.Response, true, cancellationToken);

                    // Check if escalation is needed
                    if (mcpResponse.ShouldEscalate)
                    {
                        // Find available operator and assign
                        // For now, just keep with AI
                    }
                }
                else
                {
                    await SendOutgoingMessageAsync(
                        conversation.Id,
                        "I'm having trouble processing your request. Please try again later.",
                        true,
                        cancellationToken);
                }
            }
            // If with operator, operator will respond manually

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error processing message: {ex.Message}");
        }
    }

    private async Task SendOutgoingMessageAsync(Guid conversationId, string text, bool isFromAI, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation == null)
            return;

        var messageContent = MessageContent.Create(text);
        var outgoingMessage = Message.Create(
            conversationId,
            messageContent,
            MessageType.Text,
            MessageDirection.Outgoing,
            isFromAI);

        await _messageRepository.AddAsync(outgoingMessage, cancellationToken);

        // Send via WhatsApp
        var whatsAppMessageId = await _whatsAppService.SendMessageAsync(
            conversation.PhoneNumber.Value,
            text,
            cancellationToken);

        outgoingMessage.SetWhatsAppMessageId(whatsAppMessageId);
        await _messageRepository.UpdateAsync(outgoingMessage, cancellationToken);
    }
}
