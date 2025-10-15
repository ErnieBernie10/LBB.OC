using LBB.Core.Mediator;
using LBB.Reservation.Application.Shared.Exceptions;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.Extensions.Logging;
using OrchardCore.Email;

namespace LBB.Reservation.Application.Features.SessionFeature.Events;

public record ReservationCancelled(int ReservationId) : INotification;

public class ReservationCancelledOutboxHandler(
    LbbDbContext context,
    IEmailService emailService,
    ILogger<ReservationCancelledOutboxHandler> logger
) : IOutboxNotificationHandler<ReservationCancelled>
{
    public async Task HandleAsync(
        ReservationCancelled command,
        CancellationToken cancellationToken = default
    )
    {
        var reservation = await context.Reservations.FindAsync(command.ReservationId);

        if (reservation == null)
        {
            logger.LogError("Reservation does not exist");
            return;
        }

        var result = await emailService.SendAsync(
            new MailMessage()
            {
                To = reservation.Email,
                Subject = "Reservation cancelled",
                Body = "Reservation cancelled",
            }
        );
        if (!result.Succeeded)
        {
            throw new EmailException(result);
        }
    }
}
