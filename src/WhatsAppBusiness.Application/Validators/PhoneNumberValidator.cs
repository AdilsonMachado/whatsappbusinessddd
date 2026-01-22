using FluentValidation;
using System.Text.RegularExpressions;

namespace WhatsAppBusiness.Application.Validators;

/// <summary>
/// Validator for phone numbers
/// </summary>
public class PhoneNumberValidator : AbstractValidator<string>
{
    private static readonly Regex PhoneNumberRegex = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);

    public PhoneNumberValidator()
    {
        RuleFor(phoneNumber => phoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Must(BeValidPhoneNumber)
            .WithMessage("Phone number must be in international format (e.g., +1234567890)");
    }

    private bool BeValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var normalized = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        return PhoneNumberRegex.IsMatch(normalized);
    }
}
