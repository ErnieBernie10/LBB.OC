using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Commands;
using LBB.Reservation.Domain.Contracts.Repository;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class UpdateSessionInfoCommand : IUpdateSessionInfoCommand, ICommand<Result>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public int Capacity { get; set; }

    public int SessionId { get; set; }
}

public class UpdateSessionInfoCommandHandler(IUnitOfWork uow, ISessionRepository repository)
    : ICommandHandler<UpdateSessionInfoCommand, Result>
{
    public async Task<Result> HandleAsync(
        UpdateSessionInfoCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await repository.FindById(command.SessionId);
        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        session.UpdateInfo(command);

        uow.RegisterChange(session);

        await uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
