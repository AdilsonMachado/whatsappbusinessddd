namespace WhatsAppBusiness.Infrastructure.Configuration.Models;

/// <summary>
/// MCP (Model Context Protocol) server settings
/// </summary>
public class MCPSettings
{
    /// <summary>
    /// Gets or sets the MCP server URL
    /// </summary>
    public string ServerUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MCP API key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
}
