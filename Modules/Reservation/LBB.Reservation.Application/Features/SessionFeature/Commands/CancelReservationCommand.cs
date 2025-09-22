using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Events;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class CancelReservationCommand : ICommand<Result>
{
    public int ReservationId { get; set; }
    public int SessionId { get; set; }
}

public class ReservationCancelledCommandHandler(LbbDbContext context, IOutboxService outboxService)
    : ICommandHandler<CancelReservationCommand, Result>
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

        await outboxService.PublishAsync(
            nameof(Reservation),
            reservation.Id.ToString(),
            new ReservationCancelled(reservation.Id),
            cancellationToken
        );

        await context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
