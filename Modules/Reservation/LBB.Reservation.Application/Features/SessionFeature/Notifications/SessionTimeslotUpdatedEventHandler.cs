using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;
using LBB.Reservation.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class SessionTimeslotUpdatedEventHandler(LbbDbContext context)
    : INotificationHandler<SessionTimeslotUpdatedEvent>
{
    public async Task HandleAsync(
        SessionTimeslotUpdatedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await context.Sessions.FindAsync(command.Session.Id);
        if (session == null)
            throw new ArgumentException("Session is null");

        session.Start = command.Session.Timeslot.Value.Start;
        session.End = command.Session.Timeslot.Value.End;
    }
}
