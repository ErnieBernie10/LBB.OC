using System.ComponentModel.DataAnnotations;
using System.Data;
using FluentResults;
using FluentValidation;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Core.ValueObjects;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Commands;
using LBB.Reservation.Infrastructure;
using Microsoft.Extensions.Localization;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public class AddReservationCommand : IAddReservationCommand, ICommand<Result<int>>
{
    public required string Firstname { get; set; }

    public required string Lastname { get; set; }
    public int? AttendeeCount { get; set; }

    public required string Email { get; set; } = "";

    public required string PhoneNumber { get; set; } = "";
    public int SessionId { get; set; }
}

public class AddReservationCommandValidator : AbstractValidator<AddReservationCommand>
{
    public AddReservationCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().MaximumLength(DbConstraints.Reservation.MaxEmailLength);
        RuleFor(x => x.PhoneNumber).MaximumLength(DbConstraints.Reservation.MaxPhoneNumberLength);
        RuleFor(x => x.Firstname).MaximumLength(DbConstraints.Reservation.MaxFirstnameLength);
        RuleFor(x => x.Lastname).MaximumLength(DbConstraints.Reservation.MaxLastnameLength);
        RuleFor(x => x.AttendeeCount).GreaterThanOrEqualTo(1);
    }
}

public class AddReservationCommandHandler(
    IUnitOfWork unitOfWork,
    IAggregateStore<Session, int> store,
    IValidator<AddReservationCommand> validator
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

        Session? session = null;
        if (!validationResult.IsFailed)
        {
            session = await store.GetByIdAsync(command.SessionId);
            if (session == null)
                return Result.Fail(new NotFoundError("Session not found"));
        }

        if (session is null)
            return validationResult;

        var addReservationResult = session!.AddReservation(command);
        var result = Result.Merge(validationResult, addReservationResult);
        if (result.IsFailed)
            return result;

        unitOfWork.RegisterChange(session);

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Ok(session.Id);
    }
}
