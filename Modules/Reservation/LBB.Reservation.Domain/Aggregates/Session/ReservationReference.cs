using System.Runtime.CompilerServices;
using System.Text;
using FluentResults;
using LBB.Core.Errors;
using LBB.Core.ValueObjects;

namespace LBB.Reservation.Domain.Aggregates.Session;

public class ReservationReference : ValueObject<ReservationReference, string>
{
    private static readonly ThreadLocal<Random> _random = new(() => new Random());

    public const short TokenSize = 8;
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    private ReservationReference(string value)
    {
        Value = value;
    }

    private static string GenerateReference()
    {
        return new string(
            Enumerable
                .Range(0, TokenSize)
                .Select(_ => _chars[_random.Value!.Next(_chars.Length)])
                .ToArray()
        );
    }

    public static ReservationReference New => new(GenerateReference());

    public override string Value { get; }

    public override string ToString()
    {
        return Value;
    }

    public static Result<ReservationReference> Parse(
        string reservationReference,
        string reservationReferencePropertyName
    )
    {
        var errors = new List<IError>();
        if (string.IsNullOrEmpty(reservationReference))
            errors.Add(new NotEmptyError(reservationReferencePropertyName));

        if (reservationReference.Length != TokenSize)
            errors.Add(new InvalidTokenError(reservationReferencePropertyName, TokenSize));

        var invalidChars = reservationReference.Where(c => !_chars.Contains(c)).ToList();
        if (invalidChars.Any())
            errors.Add(new InvalidCharacterError(reservationReferencePropertyName));

        return new ReservationReference(reservationReference);
    }
}

public class InvalidTokenError : DomainValidationError
{
    public InvalidTokenError(string propertyName, int tokenSize)
        : base(
            propertyName,
            $"Invalid reservation reference. Token should be {tokenSize} characters long."
        ) { }

    public override string ErrorCode => nameof(InvalidTokenError);
}

public class InvalidCharacterError : DomainValidationError
{
    public InvalidCharacterError(string propertyName)
        : base(propertyName, $"Reservation reference contains invalid characters.") { }

    public override string ErrorCode => nameof(InvalidCharacterError);
}
