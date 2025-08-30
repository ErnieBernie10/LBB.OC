using LBB.Core.Contracts;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class SessionInfoUpdatedEventHandler(IUnitOfWork uow)
    : INotificationHandler<SessionInfoUpdatedEvent>
{
    public Task HandleAsync(
        SessionInfoUpdatedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
