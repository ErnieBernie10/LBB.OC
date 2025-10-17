using System.ComponentModel.DataAnnotations;
using System.Data;
using FluentResults;
using FluentValidation;
using LBB.Core;
using LBB.Core.Contracts;
using LBB.Core.Enums;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Core.ValueObjects;
using LBB.Reservation.Application.Features.SessionFeature.Errors;
using LBB.Reservation.Application.Features.SessionFeature.Events;
using LBB.Reservation.Application.Features.SessionFeature.Framework;
using LBB.Reservation.Domain;
using LBB.Reservation.Infrastructure;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Session = LBB.Reservation.Infrastructure.DataModels.Session;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class AddReservationCommand : ICommand<Result<int>>, IReservationInput
{
    public string? Firstname { get; set; }

    public string? Lastname { get; set; }
    public int? AttendeeCount { get; set; }

    public required string Email { get; set; } = "";

    public string? PhoneNumber { get; set; }
    public int SessionId { get; set; }

    public Infrastructure.DataModels.Reservation ToDataModel()
    {
        return new Infrastructure.DataModels.Reservation()
        {
            Email = Email,
            Firstname = Firstname ?? "",
            Lastname = Lastname ?? "",
            Phone = PhoneNumber ?? "",
            SessionId = SessionId,
            Reference = ReservationReference.GenerateReference(),
            AttendeeCount = AttendeeCount ?? 1,
        };
    }
}

public class AddReservationCommandValidator : AbstractValidator<AddReservationCommand>
{
    public AddReservationCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty()
            .MaximumLength(DbConstraints.Reservation.MaxEmailLength);
        RuleFor(x => x.PhoneNumber).MaximumLength(DbConstraints.Reservation.MaxPhoneNumberLength);
        RuleFor(x => x.Firstname).MaximumLength(DbConstraints.Reservation.MaxFirstnameLength);
        RuleFor(x => x.Lastname).MaximumLength(DbConstraints.Reservation.MaxLastnameLength);
        RuleFor(x => x.AttendeeCount).GreaterThanOrEqualTo(1);
    }
}

public class AddReservationCommandHandler(
    IValidator<AddReservationCommand> validator,
    LbbDbContext context,
    EventDispatcherService eventDispatcherService
) : ICommandHandler<AddReservationCommand, Result<int>>
{
    public async Task<Result<int>> HandleAsync(
        AddReservationCommand command,
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

        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        var ruleset = new BusinessRuleSet<ReservationRuleContext>(
            new ReservationRules.CanReserveRule(),
            new ReservationRules.NoExistingReservationWithEmail()
        );
        var rulesResult = await ruleset.ValidateAsync(
            new ReservationRuleContext(command, session, EditType.Add),
            cancellationToken
        );
        if (rulesResult.IsFailed)
            return rulesResult;

        await Persist(command, cancellationToken);

        return Result.Ok(session.Id);
    }

    private async Task Persist(AddReservationCommand command, CancellationToken cancellationToken)
    {
        var reservation = command.ToDataModel();

        var tran = await context.Database.BeginTransactionAsync(cancellationToken);

        context.Reservations.Add(reservation);
        await context.SaveChangesAsync(cancellationToken);

        await eventDispatcherService
            .To(DispatchTarget.Outbox | DispatchTarget.RealtimeHub)
            .DispatchAsync(new ReservationAdded(reservation.Id));

        await context.SaveChangesAsync(cancellationToken);

        await tran.CommitAsync(cancellationToken);
    }
}
