using System.Data;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Contracts.Repository;

namespace LBB.Reservation.Infrastructure;

public class SessionRepository : ISessionRepository
{
    public async Task<Session?> FindById(int id)
    {
        throw new NotImplementedException();
    }
}
