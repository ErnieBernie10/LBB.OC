using FluentResults;
using LBB.Reservation.Domain.Aggregates.Session.Errors;
using LBB.Reservation.Domain.Contracts.Policy;

namespace LBB.Reservation.Domain.Aggregates.Session.Policies;

public class IndividualSessionReservationPolicy : IReservationPolicy
{
    public Result CanBook(Session session)
    {
        if (session.Reservations.Count >= 1 || session.Capacity.IsFull)
            return Result.Fail(new SessionCapacityExceededError(nameof(session.Capacity)));

        return Result.Ok();
    }
}
