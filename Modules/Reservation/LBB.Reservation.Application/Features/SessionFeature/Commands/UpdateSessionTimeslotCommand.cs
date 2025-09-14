using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Commands;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class UpdateSessionTimeslotCommand : ICommand<Result>, IUpdateSessionTimeSlotCommand
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class UpdateSessionTimeslotCommandHandler(
    IAggregateStore<Session, int> store,
    IUnitOfWork uow
) : ICommandHandler<UpdateSessionTimeslotCommand, Result>
{
    public async Task<Result> HandleAsync(
        UpdateSessionTimeslotCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await store.GetByIdAsync(command.Id);

        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        var result = session.UpdateTimeslot(command);
        if (result.IsFailed)
            return result;

        uow.RegisterChange(session);
        await uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
