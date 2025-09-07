using LBB.Core.Contracts;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class ReservationAddedEventHandler(LbbDbContext context) : INotificationHandler<ReservationAddedEvent>
{
    public async Task HandleAsync(
        ReservationAddedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        await context.Reservations.AddAsync(new Infrastructure.DataModels.Reservation()
        {
            Email = command.Reservation.Email,
            Firstname = command.Reservation.Name.Firstname,
            Lastname = command.Reservation.Name.Lastname,
            Phone = command.Reservation.Phone,
            SessionId = command.Session.Id,
            Reference = command.Reservation.Reference,
            AttendeeCount = command.Reservation.AttendeeCount,
        }, cancellationToken);
    }
}
