using WhatsAppBusiness.Domain.Common;

namespace WhatsAppBusiness.Domain.Entities;

/// <summary>
/// Represents the WhatsApp API configuration
/// </summary>
public class WhatsAppConfiguration : Entity
{
    /// <summary>
    /// Gets the WhatsApp Phone Number ID
    /// </summary>
    public string PhoneNumberId { get; private set; }

    /// <summary>
    /// Gets the WhatsApp API access token
    /// </summary>
    public string AccessToken { get; private set; }

    /// <summary>
    /// Gets the webhook verify token
    /// </summary>
    public string WebhookVerifyToken { get; private set; }

    /// <summary>
    /// Gets the WhatsApp Business Account ID
    /// </summary>
    public string BusinessAccountId { get; private set; }

    /// <summary>
    /// Gets the date and time when the configuration was created
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the configuration was last updated
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    private WhatsAppConfiguration()
    {
        PhoneNumberId = null!;
        AccessToken = null!;
        WebhookVerifyToken = null!;
        BusinessAccountId = null!;
    }

    private WhatsAppConfiguration(
        Guid id,
        string phoneNumberId,
        string accessToken,
        string webhookVerifyToken,
        string businessAccountId,
        DateTime createdAt)
        : base(id)
    {
        PhoneNumberId = phoneNumberId;
        AccessToken = accessToken;
        WebhookVerifyToken = webhookVerifyToken;
        BusinessAccountId = businessAccountId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new WhatsApp configuration
    /// </summary>
    /// <param name="phoneNumberId">The phone number ID</param>
    /// <param name="accessToken">The access token</param>
    /// <param name="webhookVerifyToken">The webhook verify token</param>
    /// <param name="businessAccountId">The business account ID</param>
    /// <returns>A new WhatsAppConfiguration instance</returns>
    public static WhatsAppConfiguration Create(
        string phoneNumberId,
        string accessToken,
        string webhookVerifyToken,
        string businessAccountId)
    {
        if (string.IsNullOrWhiteSpace(phoneNumberId))
            throw new ArgumentException("Phone number ID cannot be empty", nameof(phoneNumberId));

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Access token cannot be empty", nameof(accessToken));

        if (string.IsNullOrWhiteSpace(webhookVerifyToken))
            throw new ArgumentException("Webhook verify token cannot be empty", nameof(webhookVerifyToken));

        if (string.IsNullOrWhiteSpace(businessAccountId))
            throw new ArgumentException("Business account ID cannot be empty", nameof(businessAccountId));

        return new WhatsAppConfiguration(
            Guid.NewGuid(),
            phoneNumberId,
            accessToken,
            webhookVerifyToken,
            businessAccountId,
            DateTime.UtcNow);
    }

    /// <summary>
    /// Updates the access token
    /// </summary>
    public void UpdateAccessToken(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new ArgumentException("Access token cannot be empty", nameof(accessToken));

        AccessToken = accessToken;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the webhook verify token
    /// </summary>
    public void UpdateWebhookVerifyToken(string webhookVerifyToken)
    {
        if (string.IsNullOrWhiteSpace(webhookVerifyToken))
            throw new ArgumentException("Webhook verify token cannot be empty", nameof(webhookVerifyToken));

        WebhookVerifyToken = webhookVerifyToken;
        UpdatedAt = DateTime.UtcNow;
    }
}
