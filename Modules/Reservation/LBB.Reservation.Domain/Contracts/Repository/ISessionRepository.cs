using LBB.Core.Contracts;
using LBB.Reservation.Domain.Aggregates.Session;

namespace LBB.Reservation.Domain.Contracts.Repository;

public interface ISessionRepository : IRepository
{
    Task<Session?> FindById(int id);
}
