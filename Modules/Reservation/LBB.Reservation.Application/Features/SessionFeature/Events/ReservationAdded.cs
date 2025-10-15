using FluentResults;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Shared.Exceptions;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.Extensions.Logging;
using OrchardCore.Email;

namespace LBB.Reservation.Application.Features.SessionFeature.Events;

public record ReservationAdded(int ReservationId) : INotification;

public class ReservationAddedOutboxHandler(
    IEmailService emailService,
    LbbDbContext context,
    ILogger<ReservationAddedOutboxHandler> logger
) : IOutboxNotificationHandler<ReservationAdded>
{
    public async Task HandleAsync(
        ReservationAdded command,
        CancellationToken cancellationToken = default
    )
    {
        var reservation = await context.Reservations.FindAsync(command.ReservationId);
        if (reservation == null)
        {
            logger.LogError("Reservation does not exist");
            return;
        }

        if (reservation.ConfirmationSentOn.HasValue)
            return;

        var result = await emailService.SendAsync(
            new MailMessage()
            {
                To = reservation.Email,
                Subject = "Reservation created",
                Body = "Reservation created",
            }
        );
        if (!result.Succeeded)
        {
            throw new EmailException(result);
        }

        reservation.ConfirmationSentOn = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
    }
}
