namespace WhatsAppBusiness.Domain.ValueObjects;

/// <summary>
/// Represents the type of message content
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Text message
    /// </summary>
    Text,

    /// <summary>
    /// Image message
    /// </summary>
    Image,

    /// <summary>
    /// Audio message
    /// </summary>
    Audio,

    /// <summary>
    /// Video message
    /// </summary>
    Video,

    /// <summary>
    /// Document message
    /// </summary>
    Document,

    /// <summary>
    /// Location message
    /// </summary>
    Location,

    /// <summary>
    /// Contact message
    /// </summary>
    Contact,

    /// <summary>
    /// Sticker message
    /// </summary>
    Sticker
}
