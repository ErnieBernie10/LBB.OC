using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Commands;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class UpdateSessionInfoCommand : IUpdateSessionInfoCommand, ICommand<Result>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public int Capacity { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public int SessionId { get; set; }
}

public class UpdateSessionInfoCommandHandler(IUnitOfWork uow, IAggregateStore<Session, int> store)
    : ICommandHandler<UpdateSessionInfoCommand, Result>
{
    public async Task<Result> HandleAsync(
        UpdateSessionInfoCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await store.GetByIdAsync(command.SessionId);
        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        var result = session.UpdateInfo(command);
        if (result.IsFailed)
            return result;

        uow.RegisterChange(session);

        await uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
