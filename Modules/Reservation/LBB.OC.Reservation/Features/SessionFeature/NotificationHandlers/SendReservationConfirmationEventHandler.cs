using FluentResults;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using OrchardCore.Email;
using ReservationConfirmationSentEvent = LBB.Reservation.Application.Features.SessionFeature.Commands.ReservationConfirmationSentEvent;

namespace LBB.OC.Reservation.Features.SessionFeature.NotificationHandlers;

public class SendReservationConfirmationEventHandler(IEmailService emailService, IMediator mediator)
    : IOutboxNotificationHandler<ReservationAddedEvent>
{
    public async Task HandleAsync(
        ReservationAddedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        await emailService.SendAsync(
            new MailMessage()
            {
                To = command.Reservation.Email,
                Subject = "Reservation created",
                Body = "Reservation created",
            }
        );

        await mediator.SendCommandAsync<ReservationConfirmationSentEvent, Result>(
            new ReservationConfirmationSentEvent(command.Session.Id, command.Reservation.Id),
            cancellationToken
        );
    }
}
