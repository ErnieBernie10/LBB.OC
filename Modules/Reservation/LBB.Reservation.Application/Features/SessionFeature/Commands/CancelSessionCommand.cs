using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Events;
using LBB.Reservation.Infrastructure.Context;
using LBB.Reservation.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class CancelSessionCommand : ICommand<Result>
{
    public int SessionId { get; set; }
}

public class CancelSessionCommandHandler(LbbDbContext context, IOutboxService outboxService)
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
            return Result.Fail(new Error("Session has already started"));

        var now = DateTime.UtcNow;
        foreach (var reservation in session.Reservations)
        {
            reservation.CancelledOn = now;
        }
        session.CancelledOn = now;

        await Persist(cancellationToken, session);

        return Result.Ok();
    }

    private async Task Persist(CancellationToken cancellationToken, Session session)
    {
        await outboxService.PublishAsync(
            nameof(Session),
            session.Id.ToString(),
            new SessionCancelled(session.Id),
            cancellationToken
        );

        await context.SaveChangesAsync(cancellationToken);
    }
}
