using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Application.Interfaces;

namespace WhatsAppBusiness.Infrastructure.WhatsApp;

/// <summary>
/// Service for interacting with WhatsApp Business API
/// </summary>
public class WhatsAppService : IWhatsAppService
{
    private readonly WhatsAppClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WhatsAppService> _logger;

    public WhatsAppService(
        WhatsAppClient client,
        IConfiguration configuration,
        ILogger<WhatsAppService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<string> SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        var phoneNumberId = _configuration["WhatsApp:PhoneNumberId"] ?? throw new InvalidOperationException("WhatsApp Phone Number ID not configured");
        
        _logger.LogInformation("Sending WhatsApp message to {PhoneNumber}", phoneNumber);
        
        try
        {
            var messageId = await _client.SendMessageAsync(phoneNumberId, phoneNumber, message, cancellationToken);
            
            _logger.LogInformation("WhatsApp message sent successfully. MessageId: {MessageId}", messageId);
            
            return messageId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send WhatsApp message to {PhoneNumber}", phoneNumber);
            throw;
        }
    }

    /// <inheritdoc/>
    public Task ProcessIncomingMessageAsync(WhatsAppMessageDto whatsAppMessage, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing incoming WhatsApp message from {From}", whatsAppMessage.From);
        
        // This is typically handled by the command handler
        // Implementation can be added if needed
        
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task MarkAsDeliveredAsync(string whatsAppMessageId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Marking message {MessageId} as delivered", whatsAppMessageId);
        
        // Implementation would call WhatsApp API to update message status
        // For now, this is a placeholder
        
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task MarkAsReadAsync(string whatsAppMessageId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Marking message {MessageId} as read", whatsAppMessageId);
        
        // Implementation would call WhatsApp API to update message status
        // For now, this is a placeholder
        
        return Task.CompletedTask;
    }
}
