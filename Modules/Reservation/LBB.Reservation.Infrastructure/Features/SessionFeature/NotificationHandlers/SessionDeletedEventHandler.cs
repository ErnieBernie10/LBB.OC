using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Infrastructure.Features.SessionFeature.NotificationHandlers;

public class SessionDeletedEventHandler(LbbDbContext context)
    : INotificationHandler<SessionDeletedEvent>
{
    public Task HandleAsync(
        SessionDeletedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        var reservations = context.Reservations.Where(r => r.SessionId == command.Session.Id);
        context.Reservations.RemoveRange(reservations);

        var sessions = context.Sessions.Where(s => s.Id == command.Session.Id);

        context.Sessions.RemoveRange(sessions);

        return Task.CompletedTask;
    }
}
