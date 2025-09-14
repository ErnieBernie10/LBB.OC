using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Commands;
using LBB.Reservation.Domain.Contracts.Repository;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class UpdateSessionTimeslotCommand : ICommand<Result>, IUpdateSessionTimeSlotCommand
{
    public int Id { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class UpdateSessionTimeslotCommandHandler(ISessionRepository repository, IUnitOfWork uow)
    : ICommandHandler<UpdateSessionTimeslotCommand, Result>
{
    public async Task<Result> HandleAsync(
        UpdateSessionTimeslotCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await repository.FindById(command.Id);

        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        session.UpdateTimeslot(command);

        uow.RegisterChange(session);
        await uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
