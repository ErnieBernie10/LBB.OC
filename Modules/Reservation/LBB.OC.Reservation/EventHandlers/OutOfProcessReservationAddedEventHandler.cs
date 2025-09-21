using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using OrchardCore.Email;

namespace LBB.OC.Reservation.EventHandlers;

public class OutOfProcessReservationAddedEventHandler(IEmailService emailService)
    : IOutOfProcessNotificationHandler<ReservationAddedEvent>
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
    }
}
