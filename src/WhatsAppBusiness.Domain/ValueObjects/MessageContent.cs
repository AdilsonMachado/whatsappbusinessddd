using WhatsAppBusiness.Domain.Common;

namespace WhatsAppBusiness.Domain.ValueObjects;

/// <summary>
/// Represents message content with validation
/// </summary>
public sealed class MessageContent : ValueObject
{
    private const int MaxLength = 4096;

    /// <summary>
    /// Gets the content value
    /// </summary>
    public string Value { get; }

    private MessageContent(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new message content
    /// </summary>
    /// <param name="content">The message content</param>
    /// <returns>A new MessageContent instance</returns>
    /// <exception cref="ArgumentException">Thrown when content is invalid</exception>
    public static MessageContent Create(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty", nameof(content));

        if (content.Length > MaxLength)
            throw new ArgumentException($"Message content cannot exceed {MaxLength} characters", nameof(content));

        return new MessageContent(content.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(MessageContent content) => content.Value;
}
