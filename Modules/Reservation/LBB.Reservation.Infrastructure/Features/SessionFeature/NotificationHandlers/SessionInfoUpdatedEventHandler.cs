using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Infrastructure.Features.SessionFeature.NotificationHandlers;

public class SessionInfoUpdatedEventHandler(LbbDbContext context)
    : INotificationHandler<SessionInfoUpdatedEvent>
{
    public async Task HandleAsync(
        SessionInfoUpdatedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await context.Sessions.FindAsync(command.Session.Id);
        if (session == null)
            throw new ArgumentException("Session is null");

        session.Title = command.Session.Title;
        session.Description = command.Session.Description;
        session.Capacity = command.Session.Capacity.Max;
        session.Location = command.Session.Location;
        session.Start = command.Session.Timeslot.Start;
        session.End = command.Session.Timeslot.End;
    }
}
