namespace WhatsAppBusiness.Infrastructure.Configuration.Models;

/// <summary>
/// Root application configuration model
/// </summary>
public class AppConfiguration
{
    /// <summary>
    /// Gets or sets WhatsApp settings
    /// </summary>
    public WhatsAppSettings WhatsApp { get; set; } = new();

    /// <summary>
    /// Gets or sets database settings
    /// </summary>
    public DatabaseSettings Database { get; set; } = new();

    /// <summary>
    /// Gets or sets MCP settings
    /// </summary>
    public MCPSettings MCP { get; set; } = new();
}
