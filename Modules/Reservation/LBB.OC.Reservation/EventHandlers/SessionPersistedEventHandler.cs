using LBB.Core.Contracts;
using LBB.Core.Enums;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using OrchardCore.Email;

namespace LBB.OC.Reservation.EventHandlers;

public class SessionPersistedEventHandler(IEmailService service)
    : INotificationHandler<SessionPersistedEvent>
{
    public async Task HandleAsync(
        SessionPersistedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        if (command.State == PersistenceState.Deleted)
        {
            var emailTasks = command.Session.Reservations.Select(r =>
                service.SendAsync(
                    new MailMessage()
                    {
                        To = r.Email,
                        Subject = "Session cancelled",
                        Body = "Session cancelled",
                    }
                )
            );

            await Task.WhenAll(emailTasks);
        }
    }
}
