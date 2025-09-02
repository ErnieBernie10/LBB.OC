using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class SessionCreatedEventHandler
    : INotificationHandler<SessionCreatedEvent>
{
    public async Task HandleAsync(
        SessionCreatedEvent command,
        CancellationToken cancellationToken = default
    )
    {

    }
}
