using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Dtos;
using LBB.Reservation.Domain;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Application.Features.SessionFeature.Queries;

public class GetSessionsQuery : IQuery<IEnumerable<GetSessionsResponseDto>>
{
    public DateTimeOffset? Start { get; set; }
    public DateTimeOffset? End { get; set; }
}

public class GetSessionsQueryHandler(LbbDbContext context)
    : IQueryHandler<GetSessionsQuery, IEnumerable<GetSessionsResponseDto>>
{
    public async Task<IEnumerable<GetSessionsResponseDto>> HandleAsync(
        GetSessionsQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var start = (query.Start ?? DateTimeOffset.UtcNow.AddMonths(-1)).DateTime;
        var end = (query.End ?? DateTimeOffset.UtcNow.AddMonths(1)).DateTime;
        var sessions = await context
            .Sessions.Where(s => s.Start >= start && s.Start <= end)
            .Select(s => new GetSessionsResponseDto()
            {
                Title = s.Title,
                Start = s.Start,
                End = s.End,
                Description = s.Description,
                AttendeeCount = s.Reservations.Sum(r => r.AttendeeCount),
                Location = s.Location,
                Capacity = s.Capacity,
                Type = (Enums.SessionType)s.Type,
                Id = s.Id,
            })
            .ToListAsync(cancellationToken);

        return sessions;
    }
}
