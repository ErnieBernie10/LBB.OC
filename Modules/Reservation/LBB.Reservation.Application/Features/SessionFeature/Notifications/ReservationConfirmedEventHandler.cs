using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class ReservationConfirmedEventHandler(LbbDbContext context)
    : IInProcessNotificationHandler<ReservationConfirmedEvent>
{
    public async Task HandleAsync(
        ReservationConfirmedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        var reservation = await context.Reservations.FindAsync(command.Reservation.Id);
        if (reservation == null)
            throw new ArgumentException("Reservation is null");

        reservation.ConfirmationSentOn = DateTime.UtcNow;
    }
}
