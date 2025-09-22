using LBB.Core.Contracts;
using LBB.Core.ValueObjects;
using LBB.Reservation.Domain;
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

        var timeslot = new Timeslot(efSession.Start, efSession.End);
        var capacity = new Capacity(efSession.Capacity, efSession.Reservations.Count);
        var reservations = efSession.Reservations.Select(r =>
        {
            var reference = new ReservationReference(r.Reference);
            var email = new EmailAddress(r.Email);
            var phoneNumber = new PhoneNumber(r.Phone);
            return new Domain.Aggregates.Session.Reservation(
                id: r.Id,
                reference: reference,
                name: new PersonName(r.Firstname, r.Lastname),
                attendeeCount: r.AttendeeCount,
                email: email,
                phone: phoneNumber,
                confirmationSent: r.ConfirmationSentOn != null
            );
        });
        var session = new Session(
            efSession.Id,
            (Enums.SessionType)efSession.Type,
            timeslot,
            efSession.Title,
            efSession.Description,
            efSession.Location,
            capacity,
            reservations.ToList()
        );

        return session;
    }
}
