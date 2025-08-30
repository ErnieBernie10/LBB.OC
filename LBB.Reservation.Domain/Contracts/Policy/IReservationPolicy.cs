using FluentResults;
using LBB.Reservation.Domain.Aggregates.Session;

namespace LBB.Reservation.Domain.Contracts.Policy;

public interface IReservationPolicy
{
    Result CanBook(Session session);
}
