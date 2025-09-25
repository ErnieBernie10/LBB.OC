namespace LBB.Reservation.Application.Features.SessionFeature.Framework;

public static class ReservationReference
{
    private static readonly ThreadLocal<Random> _random = new(() => new Random());

    public const short TokenSize = 8;
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateReference()
    {
        return new string(
            Enumerable
                .Range(0, TokenSize)
                .Select(_ => _chars[_random.Value!.Next(_chars.Length)])
                .ToArray()
        );
    }
}
