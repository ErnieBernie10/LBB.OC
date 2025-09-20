using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class DeleteSessionCommand : ICommand<Result>
{
    public int SessionId { get; set; }
}

public class DeleteSessionCommandHandler(
    IAggregateStore<Session, int> store,
    IUnitOfWork unitOfWork
) : ICommandHandler<DeleteSessionCommand, Result>
{
    public async Task<Result> HandleAsync(
        DeleteSessionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await store.GetByIdAsync(command.SessionId);
        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        var result = session.Delete();
        unitOfWork.RegisterChange(session);
        await unitOfWork.CommitAsync(cancellationToken);
        return result;
    }
}
