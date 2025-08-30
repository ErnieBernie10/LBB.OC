using FluentResults;
using LBB.Reservation.Domain.Contracts.Policy;

namespace LBB.Reservation.Domain.Aggregates.Session.Policies;

public class GroupSessionReservationPolicy : IReservationPolicy
{
    public Result CanBook(Session session)
    {
        if (session.Capacity.IsFull)
            return Result.Fail("Session is full");

        return Result.Ok();
    }
}
