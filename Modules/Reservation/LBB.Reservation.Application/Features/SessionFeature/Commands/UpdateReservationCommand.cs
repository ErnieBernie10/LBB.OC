using FluentResults;
using FluentValidation;
using LBB.Core;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Events;
using LBB.Reservation.Application.Features.SessionFeature.Framework;
using LBB.Reservation.Infrastructure;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class UpdateReservationCommand : ICommand<Result>, IReservationInput
{
    public string? Firstname { get; set; }

    public string? Lastname { get; set; }
    public int? AttendeeCount { get; set; }

    public required string Email { get; set; } = "";

    public string? PhoneNumber { get; set; }
    public int ReservationId { get; set; }
    public int SessionId { get; set; }
}

public class UpdateReservationCommandValidator : AbstractValidator<UpdateReservationCommand>
{
    public UpdateReservationCommandValidator()
    {
        Include(new ReservationInputValidator());
    }
}

public class UpdateReservationCommandHandler(
    LbbDbContext context,
    IValidator<UpdateReservationCommand> validator,
    IOutboxService outboxService
) : ICommandHandler<UpdateReservationCommand, Result>
{
    public async Task<Result> HandleAsync(
        UpdateReservationCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var validation = validator.Validate(command);
        var result = validation.IsValid
            ? Result.Ok()
            : Result.Fail(new ValidationError(validation));
        if (result.IsFailed)
            return result;

        var session = await context
            .Sessions.Include(s => s.Reservations)
            .FirstOrDefaultAsync(
                s => s.Id == command.SessionId,
                cancellationToken: cancellationToken
            );
        if (session is null)
            return Result.Fail(new NotFoundError($"Session with id {command.SessionId} not found"));

        var reservation = session.Reservations.FirstOrDefault(r => r.Id == command.ReservationId);
        if (reservation is null)
            return Result.Fail(
                new NotFoundError($"Reservation with id {command.ReservationId} not found")
            );

        var ruleContext = new ReservationRuleContext(command, session);
        var ruleSet = new BusinessRuleSet<ReservationRuleContext>(
            new ReservationRules.CanReserveRule()
        );
        var rulesResult = await ruleSet.ValidateAsync(ruleContext, cancellationToken);
        if (rulesResult.IsFailed)
            return rulesResult;

        reservation.Email = command.Email;
        reservation.Phone = command.PhoneNumber ?? "";
        reservation.Firstname = command.Firstname ?? "";
        reservation.Lastname = command.Lastname ?? "";
        reservation.AttendeeCount = command.AttendeeCount ?? 1;

        await outboxService.PublishAsync(
            nameof(Reservation),
            reservation.Id.ToString(),
            new ReservationUpdatedEvent(reservation.Id),
            cancellationToken
        );
        await context.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
