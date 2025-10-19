using FluentResults;
using LBB.Core;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class CancelReservationCommand : ICommand<Result>
{
    public int ReservationId { get; set; }
    public int SessionId { get; set; }
}

public class ReservationCancelledCommandHandler(
    LbbDbContext context,
    EventDispatcherService eventService
) : ICommandHandler<CancelReservationCommand, Result>
{
    public async Task<Result> HandleAsync(
        CancelReservationCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var reservation = await context.Reservations.FindAsync(command.ReservationId);
        if (reservation == null)
            return Result.Fail(new NotFoundError("Reservation not found"));

        if (reservation.SessionId != command.SessionId)
            return Result.Fail(new NotFoundError("Reservation not found"));

        if (reservation.CancelledOn.HasValue)
            return Result.Fail(new NotFoundError("Reservation already cancelled"));

        reservation.CancelledOn = DateTime.UtcNow;
        reservation.UpdatedOn = DateTime.UtcNow;

        eventService
            .To(DispatchTarget.RealtimeHub | DispatchTarget.Outbox)
            .QueueMessage(new ReservationCancelled(reservation.Id), reservation.Id);
        eventService
            .To(DispatchTarget.RealtimeHub)
            .QueueMessage(new SessionUpdated(reservation.SessionId), reservation.SessionId);
        await eventService.DispatchAsync();

        await context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
