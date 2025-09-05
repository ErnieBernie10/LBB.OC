using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Events;
using LBB.Reservation.Infrastructure.Context;
using LBB.Reservation.Infrastructure.DataModels;

namespace LBB.Reservation.Application.Features.SessionFeature.Notifications;

public class SessionCreatedEventHandler(LbbDbContext dbContext)
    : INotificationHandler<SessionCreatedEvent>
{
    public async Task HandleAsync(
        SessionCreatedEvent command,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext.AddAsync(new Session()
        {
            Description = command.Session.Description,
            Title = command.Session.Title,
            Start = command.Session.Timeslot.Value.Start,
            End = command.Session.Timeslot.Value.End,
            Location = command.Session.Location,
            Capacity = command.Session.Capacity.Value.Max,
            Type = (int)command.Session.SessionType,
            UserId = ""
        }, cancellationToken);
    }
}
