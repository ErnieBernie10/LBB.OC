using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FluentResults;
using LBB.Core.Errors;

namespace LBB.Core.ValueObjects;

public sealed class PhoneNumber : ValueObject<PhoneNumber, string>
{
    public const int MaxLength = 20;

    // This regex allows for common phone number formats including:
    // - Optional + prefix
    // - Optional country code
    // - Digits, spaces, dashes, and parentheses
    private static readonly Regex PhoneRegex = new(@"^\+?[\d\s()\-]{6,20}$", RegexOptions.Compiled);

    internal PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string value, string valuePropertyName)
    {
        var val = NormalizePhoneNumber(value.Trim());
        var errors = new List<IError>();
        if (string.IsNullOrWhiteSpace(val))
            errors.Add(new NotEmptyError(valuePropertyName));

        if (value.Length > MaxLength)
            errors.Add(new LengthExceededError(valuePropertyName, MaxLength));

        if (!PhoneRegex.IsMatch(value))
            errors.Add(new InvalidPhoneNumberError(valuePropertyName));

        if (errors.Count > 0)
            return Result.Fail(errors);

        return new PhoneNumber(val);
    }

    private static string NormalizePhoneNumber(string value)
    {
        // Remove any whitespace or formatting characters, keeping only + and digits
        return string.Concat(value.Where(c => char.IsDigit(c) || c == '+'));
    }

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;

    public override string Value { get; }

    public override string ToString() => Value;
}
