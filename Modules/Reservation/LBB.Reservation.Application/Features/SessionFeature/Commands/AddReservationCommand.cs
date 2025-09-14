using System.ComponentModel.DataAnnotations;
using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Core.ValueObjects;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Commands;
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

public class AddReservationCommandHandler(
    IUnitOfWork unitOfWork,
    IAggregateStore<Session, int> store
) : ICommandHandler<AddReservationCommand, Result<int>>
{
    public async Task<Result<int>> HandleAsync(
        AddReservationCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await store.GetByIdAsync(command.SessionId);
        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        var result = session.AddReservation(command);
        if (result.IsFailed)
            return result.ToResult<int>();

        unitOfWork.RegisterChange(session);

        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Ok(session.Id);
    }
}
