using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using FluentResults;
using LBB.Core.Errors;

namespace LBB.Core.ValueObjects;

public sealed class EmailAddress : ValueObject<EmailAddress, string>
{
    public const int MaxLength = 200;

    public override string Value { get; }
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public static Result<EmailAddress> Create(string value, string valuePropertyName)
    {
        var errors = new List<IError>();
        if (string.IsNullOrWhiteSpace(value))
            errors.Add(new NotEmptyError(valuePropertyName));

        if (value.Length > MaxLength)
            errors.Add(new LengthExceededError(valuePropertyName, MaxLength));

        if (!EmailRegex.IsMatch(value))
            errors.Add(new InvalidEmailAddressError(valuePropertyName));

        if (errors.Count > 0)
            return Result.Fail(errors);

        return Result.Ok(new EmailAddress(value));
    }

    internal EmailAddress(string value)
    {
        Value = value;
    }
}
