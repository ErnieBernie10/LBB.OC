using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Infrastructure.Features.SessionFeature.NotificationHandlers;

public class ReservationRemovedEventHandler(LbbDbContext context)
    : INotificationHandler<ReservationRemovedEvent>
{
    public async Task HandleAsync(
        ReservationRemovedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        var reservation = await context.Reservations.FindAsync(command.Reservation.Id);

        if (reservation == null)
            return;

        context.Reservations.Remove(reservation);
    }
}
