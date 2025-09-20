using LBB.Core.Contracts;
using LBB.Core.Enums;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class SessionDeletedEventHandler(
    LbbDbContext context,
    IBackgroundNotificationQueue notificationQueue
) : INotificationHandler<SessionDeletedEvent>
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

        notificationQueue.Enqueue(
            new SessionPersistedEvent(command.Session, PersistenceState.Deleted)
        );

        return Task.CompletedTask;
    }
}
