using LBB.Core.Contracts;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class ReservationAddedEventHandler : INotificationHandler<ReservationAddedEvent>
{
    public Task HandleAsync(
        ReservationAddedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Reservation Added!");
        return Task.CompletedTask;
    }
}
