using FluentResults;
using LBB.Core;
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

public class DeleteSessionCommandHandler(LbbDbContext context, EventDispatcherService eventService)
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

        context.Sessions.Remove(session);
        eventService
            .To(DispatchTarget.Outbox | DispatchTarget.RealtimeHub)
            .QueueMessage(new SessionDeleted(session.Id), session.Id);
        eventService
            .To(DispatchTarget.RealtimeHub)
            .QueueMessage(new SessionUpdated(session.Id), session.Id);
        await eventService.DispatchAsync();
        await context.SaveChangesAsync();

        return Result.Ok();
    }
}
