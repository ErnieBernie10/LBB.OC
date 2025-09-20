using LBB.Core.Contracts;
using LBB.Core.Enums;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class ReservationAddedEventHandler(
    LbbDbContext context,
    IBackgroundNotificationQueue notificationQueue
) : INotificationHandler<ReservationAddedEvent>
{
    public async Task HandleAsync(
        ReservationAddedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        await context.Reservations.AddAsync(
            new Infrastructure.DataModels.Reservation()
            {
                Email = command.Reservation.Email.Value,
                Firstname = command.Reservation.Name?.Firstname!,
                Lastname = command.Reservation.Name?.Lastname!,
                Phone = command.Reservation.Phone?.Value!,
                SessionId = command.Session.Id,
                Reference = command.Reservation.Reference,
                AttendeeCount = command.Reservation.AttendeeCount,
            },
            cancellationToken
        );

        notificationQueue.Enqueue(
            new ReservationPersistedEvent(command.Reservation, PersistenceState.Added)
        );
    }
}
