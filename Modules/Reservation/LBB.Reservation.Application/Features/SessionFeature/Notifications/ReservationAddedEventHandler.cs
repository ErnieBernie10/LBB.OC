using LBB.Core.Contracts;
using LBB.Core.Enums;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class InProcessReservationAddedEventHandler(LbbDbContext context, IOutboxService service)
    : IInProcessNotificationHandler<ReservationAddedEvent>
{
    public async Task HandleAsync(
        ReservationAddedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        await context.Reservations.AddAsync(
            new Infrastructure.DataModels.Reservation()
            {
                Email = command.Reservation.Email,
                Firstname = command.Reservation.Firstname ?? "",
                Lastname = command.Reservation.Lastname ?? "",
                Phone = command.Reservation.Phone ?? "",
                SessionId = command.Session.Id,
                Reference = command.Reservation.Reference,
                AttendeeCount = command.Reservation.AttendeeCount,
            },
            cancellationToken
        );
    }
}

public class OutOfProcessReservationAddedEventHandler
    : IOutOfProcessNotificationHandler<ReservationAddedEvent>
{
    public Task HandleAsync(
        ReservationAddedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        Console.WriteLine("Executed");
        return Task.CompletedTask;
    }
}
