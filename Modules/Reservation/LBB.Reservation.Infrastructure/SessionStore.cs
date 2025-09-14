using LBB.Core.Contracts;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure;

public class SessionStore(LbbDbContext context) : IAggregateStore<Session, int>
{
    public async Task<Session?> GetByIdAsync(int id)
    {
        var efSession = await context
            .Sessions.Include(s => s.Reservations)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (efSession == null)
            return null;

        return new Session(
            efSession.Id,
            efSession.Type,
            efSession.Start,
            efSession.End,
            efSession.Title,
            efSession.Description,
            efSession.Location,
            efSession.Capacity,
            efSession
                .Reservations.Select(r => new Domain.Aggregates.Session.Reservation(
                    r.Reference,
                    r.Firstname,
                    r.Lastname,
                    r.AttendeeCount,
                    r.Email,
                    r.Phone
                ))
                .ToList()
        );
    }
}
