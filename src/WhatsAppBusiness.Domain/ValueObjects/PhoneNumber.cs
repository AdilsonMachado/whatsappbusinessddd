using WhatsAppBusiness.Domain.Common;
using System.Text.RegularExpressions;

namespace WhatsAppBusiness.Domain.ValueObjects;

/// <summary>
/// Represents a phone number in international format
/// </summary>
public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex PhoneNumberRegex = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);

    /// <summary>
    /// Gets the phone number value
    /// </summary>
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new phone number from a string value
    /// </summary>
    /// <param name="phoneNumber">The phone number in international format (e.g., +1234567890)</param>
    /// <returns>A new PhoneNumber instance</returns>
    /// <exception cref="ArgumentException">Thrown when the phone number format is invalid</exception>
    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

        var normalizedNumber = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        if (!PhoneNumberRegex.IsMatch(normalizedNumber))
            throw new ArgumentException($"Invalid phone number format: {phoneNumber}. Expected international format (e.g., +1234567890)", nameof(phoneNumber));

        if (!normalizedNumber.StartsWith("+"))
            normalizedNumber = "+" + normalizedNumber;

        return new PhoneNumber(normalizedNumber);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}
