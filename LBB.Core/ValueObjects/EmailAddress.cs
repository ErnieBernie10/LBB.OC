using System.Text.RegularExpressions;
using FluentResults;

namespace LBB.Core.ValueObjects;

public sealed class EmailAddress : ValueObject<EmailAddress, string>
{
    public const int MaxLength = 200;
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    private EmailAddress(string value)
        : base(value.ToLower()) { }

    public static Result<EmailAddress> Create(string value)
    {
        return Validate(value, ValidateEmail, v => new EmailAddress(v));
    }

    private static Result ValidateEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail("Email address cannot be empty");

        if (value.Length > MaxLength)
            return Result.Fail($"Email address cannot be longer than {MaxLength} characters");

        if (!EmailRegex.IsMatch(value))
            return Result.Fail("Invalid email address format");

        return Result.Ok();
    }
}
