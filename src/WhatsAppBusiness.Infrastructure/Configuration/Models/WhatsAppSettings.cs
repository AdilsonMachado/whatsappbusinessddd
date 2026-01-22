namespace WhatsAppBusiness.Infrastructure.Configuration.Models;

/// <summary>
/// WhatsApp API settings
/// </summary>
public class WhatsAppSettings
{
    /// <summary>
    /// Gets or sets the WhatsApp Phone Number ID
    /// </summary>
    public string PhoneNumberId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the WhatsApp API access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the webhook verify token
    /// </summary>
    public string VerifyToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the WhatsApp Business Account ID
    /// </summary>
    public string BusinessAccountId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the WhatsApp app secret for webhook validation
    /// </summary>
    public string AppSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the WhatsApp API base URL
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://graph.facebook.com";
}
