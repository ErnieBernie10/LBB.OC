using FluentResults;
using LBB.Core;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Errors;
using LBB.Reservation.Application.Features.SessionFeature.Events;
using LBB.Reservation.Infrastructure.Context;
using LBB.Reservation.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class CancelSessionCommand : ICommand<Result>
{
    public int SessionId { get; set; }
}

public class CancelSessionCommandHandler(LbbDbContext context, EventDispatcherService eventService)
    : ICommandHandler<CancelSessionCommand, Result>
{
    public async Task<Result> HandleAsync(
        CancelSessionCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await context
            .Sessions.Include(s => s.Reservations)
            .FirstOrDefaultAsync(s => s.Id == command.SessionId, cancellationToken);

        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        if (session.Start.ToUniversalTime() < DateTime.UtcNow)
            return Result.Fail(new SessionPassedError());

        var now = DateTime.UtcNow;
        foreach (var reservation in session.Reservations)
        {
            reservation.CancelledOn = now;
        }
        session.CancelledOn = now;

        eventService
            .To(DispatchTarget.RealtimeHub | DispatchTarget.Outbox)
            .QueueMessage(new SessionCancelled(session.Id), session.Id);
        eventService
            .To(DispatchTarget.RealtimeHub)
            .QueueMessage(new SessionUpdated(session.Id), session.Id);
        await eventService.DispatchAsync();

        await context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
