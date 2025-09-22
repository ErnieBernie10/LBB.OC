using FluentResults;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Commands;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using OrchardCore.Email;

namespace LBB.OC.Reservation.Features.SessionFeature.NotificationHandlers;

public class SendReservationConfirmationEventHandler(IMediator mediator)
    : IOutboxNotificationHandler<ReservationAddedEvent>
{
    public async Task HandleAsync(
        ReservationAddedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        await mediator.SendCommandAsync<SendReservationConfirmationCommand, Result>(
            new SendReservationConfirmationCommand(command.Session.Id, command.Reservation.Id),
            cancellationToken
        );
    }
}
