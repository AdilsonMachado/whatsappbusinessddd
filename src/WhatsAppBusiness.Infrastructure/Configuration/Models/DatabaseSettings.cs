namespace WhatsAppBusiness.Infrastructure.Configuration.Models;

/// <summary>
/// Database connection settings
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// Gets or sets the database connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;
}
