using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public record ReservationConfirmationSentEvent(int SessionId, int ReservationId) : ICommand<Result>;

public class ReservationConfirmationSentCommandHandler(
    IAggregateStore<Session, int> store,
    IUnitOfWork unitOfWork
) : ICommandHandler<ReservationConfirmationSentEvent, Result>
{
    public async Task<Result> HandleAsync(
        ReservationConfirmationSentEvent command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await store.GetByIdAsync(command.SessionId);
        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        if (session.ConfirmReservation(command.ReservationId))
        {
            // TODO: Add proper error
            return Result.Fail("Reservation already confirmed");
        }
        unitOfWork.RegisterChange(session);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result.Ok();
    }
}
