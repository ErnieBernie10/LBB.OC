using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class ReservationConfirmationSentEventHandler(LbbDbContext context)
    : IInProcessNotificationHandler<ReservationConfirmationSentEvent>
{
    public async Task HandleAsync(
        ReservationConfirmationSentEvent command,
        CancellationToken cancellationToken = default
    )
    {
        var reservation = await context.Reservations.FindAsync(command.Reservation.Id);
        if (reservation == null)
            return;

        reservation.ConfirmationSentOn = DateTime.UtcNow;
    }
}
