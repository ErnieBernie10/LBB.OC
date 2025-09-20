using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class CancelReservationCommand : ICommand<Result>
{
    public int ReservationId { get; set; }
    public int SessionId { get; set; }
}

public class ReservationCancelledCommandHandler(
    IAggregateStore<Session, int> store,
    IUnitOfWork unitOfWork
) : ICommandHandler<CancelReservationCommand, Result>
{
    public async Task<Result> HandleAsync(
        CancelReservationCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await store.GetByIdAsync(command.SessionId);
        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        var result = session.CancelReservation(command.ReservationId);
        if (result.IsFailed)
            return result;

        unitOfWork.RegisterChange(session);
        await unitOfWork.CommitAsync(cancellationToken);

        return result;
    }
}
