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

    private PhoneNumber(string value)
        : base(value) { }

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail(new DomainValidationError("Phone number cannot be empty"));

        return Validate(
            value.Trim(),
            ValidatePhoneNumber,
            v => new PhoneNumber(NormalizePhoneNumber(v))
        );
    }

    private static Result ValidatePhoneNumber(string value)
    {
        if (value.Length > MaxLength)
            return Result.Fail(
                new DomainValidationError($"Phone number cannot be longer than {MaxLength} characters")
            );

        if (!PhoneRegex.IsMatch(value))
            return Result.Fail(new DomainValidationError("Invalid phone number format"));

        return Result.Ok();
    }

    private static string NormalizePhoneNumber(string value)
    {
        // Remove any whitespace or formatting characters, keeping only + and digits
        return string.Concat(value.Where(c => char.IsDigit(c) || c == '+'));
    }

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;

    public override string ToString() => Value;
}
