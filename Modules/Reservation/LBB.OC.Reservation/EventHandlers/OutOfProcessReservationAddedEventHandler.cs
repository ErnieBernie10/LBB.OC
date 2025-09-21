using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Commands;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using Microsoft.Extensions.Logging;
using OrchardCore.Email;

namespace LBB.OC.Reservation.EventHandlers;

public class OutOfProcessReservationAddedEventHandler(
    IEmailService emailService,
    ILogger<OutOfProcessReservationAddedEventHandler> logger,
    IMediator mediator
) : IOutOfProcessNotificationHandler<ReservationAddedEvent>
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
