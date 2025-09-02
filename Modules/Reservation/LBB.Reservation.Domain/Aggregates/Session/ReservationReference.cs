using System.Text;
using FluentResults;
using LBB.Core.ValueObjects;

namespace LBB.Reservation.Domain.Aggregates.Session;

public class ReservationReference : ValueObject<ReservationReference, string>
{
    private static readonly ThreadLocal<Random> _random = new(() => new Random());

    public const short TokenSize = 8;
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public ReservationReference(string value)
        : base(value) { }

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

    public override string ToString()
    {
        return Value;
    }

    public static Result<ReservationReference> Parse(string reservationReference)
    {
        if (string.IsNullOrEmpty(reservationReference))
            return Result.Fail("Reservation reference cannot be empty");

        if (reservationReference.Length != TokenSize)
            return Result.Fail(
                $"Reservation reference must be exactly {TokenSize} characters long"
            );

        var invalidChars = reservationReference.Where(c => !_chars.Contains(c)).ToList();
        if (invalidChars.Any())
            return Result.Fail(
                $"Reservation reference contains invalid characters: {string.Join(", ", invalidChars)}"
            );

        return new ReservationReference(reservationReference);
    }
}
