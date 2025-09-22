using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Infrastructure.Features.SessionFeature.NotificationHandlers;

public class ReservationConfirmationSentEventHandler(LbbDbContext context)
    : INotificationHandler<ReservationConfirmationSentEvent>
{
    public async Task HandleAsync(
        ReservationConfirmationSentEvent command,
        CancellationToken cancellationToken = default
    )
    {
        var reservation = await context.Reservations.FindAsync(command.Reservation.Id);
        if (reservation == null)
            throw new ArgumentException("Reservation is null");

        reservation.ConfirmationSentOn = DateTime.UtcNow;
    }
}
