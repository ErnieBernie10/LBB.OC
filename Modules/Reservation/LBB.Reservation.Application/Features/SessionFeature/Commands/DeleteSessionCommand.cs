using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Events;
using LBB.Reservation.Infrastructure.Context;
using Session = LBB.Reservation.Infrastructure.DataModels.Session;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class DeleteSessionCommand : ICommand<Result>
{
    public int SessionId { get; set; }
}

public class DeleteSessionCommandHandler(LbbDbContext context, IOutboxService outboxService)
    : ICommandHandler<DeleteSessionCommand, Result>
{
    public async Task<Result> HandleAsync(
        DeleteSessionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await context.Sessions.FindAsync(command.SessionId);
        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        await Persist(session);

        return Result.Ok();
    }

    private async Task Persist(Session session)
    {
        context.Sessions.Remove(session);
        await outboxService.PublishAsync(
            nameof(Session),
            session.Id.ToString(),
            new SessionDeleted(session.Id)
        );
        await context.SaveChangesAsync();
    }
}
