namespace WhatsAppBusiness.Application.DTOs;

/// <summary>
/// Data transfer object for MCP (Model Context Protocol) response
/// </summary>
public class MCPResponseDto
{
    /// <summary>
    /// Gets or sets the AI generated response text
    /// </summary>
    public string Response { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the request was successful
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the error message if any
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets whether the conversation should be escalated to a human operator
    /// </summary>
    public bool ShouldEscalate { get; set; }

    /// <summary>
    /// Gets or sets additional metadata
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();
}
