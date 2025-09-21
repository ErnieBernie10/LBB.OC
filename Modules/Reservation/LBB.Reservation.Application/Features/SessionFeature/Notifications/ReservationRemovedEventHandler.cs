using LBB.Core.Contracts;
using LBB.Core.Enums;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class ReservationRemovedEventHandler(LbbDbContext context)
    : IInProcessNotificationHandler<ReservationRemovedEvent>
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
