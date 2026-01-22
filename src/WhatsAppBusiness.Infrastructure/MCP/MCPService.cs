using Microsoft.Extensions.Logging;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Application.Interfaces;

namespace WhatsAppBusiness.Infrastructure.MCP;

/// <summary>
/// Service for interacting with MCP (Model Context Protocol) server for AI
/// </summary>
public class MCPService : IMCPService
{
    private readonly MCPClient _client;
    private readonly MCPMessageFormatter _formatter;
    private readonly ILogger<MCPService> _logger;

    public MCPService(
        MCPClient client,
        MCPMessageFormatter formatter,
        ILogger<MCPService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<MCPResponseDto> SendRequestAsync(MCPRequestDto request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending request to MCP server");
        
        try
        {
            // Format the conversation history for the AI
            var context = request.History?
                .Select(m => (
                    m.Direction == Domain.ValueObjects.MessageDirection.Incoming ? "user" : "assistant",
                    m.Content
                ))
                .ToList();

            // Send to MCP server
            var response = await _client.SendMessageAsync(
                request.Message,
                context,
                cancellationToken);

            _logger.LogInformation("Received response from MCP server");

            return new MCPResponseDto
            {
                Response = response,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send request to MCP server");
            
            return new MCPResponseDto
            {
                Response = string.Empty,
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking MCP server health");
            
            // Try to send a simple health check request
            var result = await _client.SendMessageAsync(
                "ping",
                null,
                cancellationToken);

            return !string.IsNullOrEmpty(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MCP server health check failed");
            return false;
        }
    }
}
