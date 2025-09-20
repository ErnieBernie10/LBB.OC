using LBB.Core.Enums;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using OrchardCore.Email;
using OrchardCore.Notifications;
using OrchardCore.Notifications.Models;

namespace LBB.OC.Reservation.EventHandlers;

public class ReservationPersistedEventHandler(IEmailService service)
    : INotificationHandler<ReservationPersistedEvent>
{
    public async Task HandleAsync(
        ReservationPersistedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        if (command.State == PersistenceState.Added)
        {
            await service.SendAsync(
                new MailMessage()
                {
                    To = command.Reservation.Email,
                    Body = "Reservation created",
                    Subject = "Reservation created",
                }
            );
        }
    }
}
