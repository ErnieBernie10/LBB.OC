using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Infrastructure.Features.SessionFeature.NotificationHandlers;

public class ReservationAddedEventHandler(LbbDbContext context)
    : INotificationHandler<ReservationAddedEvent>
{
    public Task HandleAsync(
        ReservationAddedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        context.Reservations.Add(
            new Infrastructure.DataModels.Reservation()
            {
                Email = command.Reservation.Email,
                Firstname = command.Reservation.Firstname ?? "",
                Lastname = command.Reservation.Lastname ?? "",
                Phone = command.Reservation.Phone ?? "",
                SessionId = command.Session.Id,
                Reference = command.Reservation.Reference,
                AttendeeCount = command.Reservation.AttendeeCount,
            }
        );
        return Task.CompletedTask;
    }
}
