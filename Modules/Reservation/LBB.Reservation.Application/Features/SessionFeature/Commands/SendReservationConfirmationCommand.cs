using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;
using OrchardCore.Email;

namespace LBB.Reservation.Application.Features.SessionFeature.Commands;

public record SendReservationConfirmationCommand(int SessionId, int ReservationId)
    : ICommand<Result>;

public class SendReservationConfirmationCommandHandler(
    IAggregateStore<Session, int> store,
    IUnitOfWork unitOfWork,
    IEmailService emailService
) : ICommandHandler<SendReservationConfirmationCommand, Result>
{
    public async Task<Result> HandleAsync(
        SendReservationConfirmationCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await store.GetByIdAsync(command.SessionId);
        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        if (session.ConfirmReservation(command.ReservationId))
        {
            // TODO: Add proper error
            return Result.Fail("Reservation already confirmed");
        }

        var reservation = session.FindReservation(command.ReservationId);
        if (reservation == null)
            return Result.Fail(new NotFoundError("Reservation not found"));

        await emailService.SendAsync(
            new MailMessage()
            {
                To = reservation.Email,
                Subject = "Reservation created",
                Body = "Reservation created",
            }
        );

        unitOfWork.RegisterChange(session);
        await unitOfWork.CommitAsync(cancellationToken);
        return Result.Ok();
    }
}
