namespace WhatsAppBusiness.Domain.ValueObjects;

/// <summary>
/// Represents the direction of a message
/// </summary>
public enum MessageDirection
{
    /// <summary>
    /// Message received from user
    /// </summary>
    Incoming,

    /// <summary>
    /// Message sent to user
    /// </summary>
    Outgoing
}
