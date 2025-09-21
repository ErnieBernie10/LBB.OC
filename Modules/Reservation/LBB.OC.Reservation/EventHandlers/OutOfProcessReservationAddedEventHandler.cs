using LBB.Core.Contracts;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using Microsoft.Extensions.Logging;
using OrchardCore.Email;

namespace LBB.OC.Reservation.EventHandlers;

public class OutOfProcessReservationAddedEventHandler(
    IEmailService emailService,
    IUnitOfWork uow,
    IAggregateStore<Session, int> store,
    ILogger<OutOfProcessReservationAddedEventHandler> logger
) : IOutOfProcessNotificationHandler<ReservationAddedEvent>
{
    public async Task HandleAsync(
        ReservationAddedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        var session = await store.GetByIdAsync(command.Session.Id);
        if (session == null)
        {
            logger.LogWarning("Session not found");
            return;
        }
        var result = session.ConfirmReservation(command.Reservation.Id);
        if (!result)
        {
            logger.LogWarning("Reservation already confirmed");
            return;
        }

        await emailService.SendAsync(
            new MailMessage()
            {
                To = command.Reservation.Email,
                Subject = "Reservation created",
                Body = "Reservation created",
            }
        );

        await uow.CommitAsync(cancellationToken);
    }
}
