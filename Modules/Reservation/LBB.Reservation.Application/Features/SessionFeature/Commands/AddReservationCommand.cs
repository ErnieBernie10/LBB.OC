using System.ComponentModel.DataAnnotations;
using System.Data;
using FluentResults;
using FluentValidation;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Core.ValueObjects;
using LBB.Reservation.Application.Features.SessionFeature.Events;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Commands;
using LBB.Reservation.Infrastructure;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class AddReservationCommand : IAddReservationCommand, ICommand<Result<int>>
{
    public string? Firstname { get; set; }

    public string? Lastname { get; set; }
    public int? AttendeeCount { get; set; }

    public required string Email { get; set; } = "";

    public string? PhoneNumber { get; set; }
    public int SessionId { get; set; }
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
    IOutboxService outboxService
) : ICommandHandler<AddReservationCommand, Result<int>>
{
    public async Task<Result<int>> HandleAsync(
        AddReservationCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var validation = validator.Validate(command);
        var validationResult = validation.IsValid
            ? Result.Ok()
            : Result.Fail(new ValidationError(validation));

        var result = Result.Merge(validationResult);

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

        // TODO: Refactor the error to not use the domain anymore
        if (session.AttendeeCount + (command.AttendeeCount ?? 1) > session.Capacity)
            return Result.Fail(new Capacity.CapacityExceededError("AttendeeCount"));

        var reservation = new Infrastructure.DataModels.Reservation()
        {
            Email = command.Email,
            Firstname = command.Firstname ?? "",
            Lastname = command.Lastname ?? "",
            Phone = command.PhoneNumber ?? "",
            SessionId = session.Id,
            Reference = ReservationReference.New,
            AttendeeCount = command.AttendeeCount ?? 1,
        };

        var tran = await context.Database.BeginTransactionAsync(cancellationToken);

        context.Reservations.Add(reservation);
        await context.SaveChangesAsync(cancellationToken);

        await outboxService.PublishAsync(
            nameof(Reservation),
            reservation.Id.ToString(),
            nameof(ReservationAdded),
            new ReservationAdded(reservation.Id),
            cancellationToken
        );
        await context.SaveChangesAsync(cancellationToken);

        await tran.CommitAsync(cancellationToken);

        return Result.Ok(session.Id);
    }
}
