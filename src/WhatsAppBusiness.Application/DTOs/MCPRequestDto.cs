namespace WhatsAppBusiness.Application.DTOs;

/// <summary>
/// Data transfer object for MCP (Model Context Protocol) request
/// </summary>
public class MCPRequestDto
{
    /// <summary>
    /// Gets or sets the conversation ID
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the user's message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the conversation history
    /// </summary>
    public List<MessageDto> History { get; set; } = new();

    /// <summary>
    /// Gets or sets the user context
    /// </summary>
    public Dictionary<string, string> Context { get; set; } = new();
}
