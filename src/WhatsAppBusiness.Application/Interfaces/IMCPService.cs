using WhatsAppBusiness.Application.DTOs;

namespace WhatsAppBusiness.Application.Interfaces;

/// <summary>
/// Interface for MCP (Model Context Protocol) service for AI interactions
/// </summary>
public interface IMCPService
{
    /// <summary>
    /// Sends a request to the MCP server and gets AI response
    /// </summary>
    /// <param name="request">The MCP request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>MCP response from AI</returns>
    Task<MCPResponseDto> SendRequestAsync(MCPRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the MCP server is healthy
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if healthy, false otherwise</returns>
    Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default);
}
