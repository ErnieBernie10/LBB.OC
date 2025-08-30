using FluentResults;
using LBB.Core.Mediator;
using LBB.Reservation.Domain;

namespace LBB.Reservation.Application.Features.SessionFeature.Queries;

public class SessionBasic
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}

public class GetSessionsPagedQuery : IQuery<Result<(int, IEnumerable<SessionBasic>)>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public DateTime? From { get; set; }
    public DateTime? Until { get; set; }
}

public class GetSessionsPagedQueryHandler
    : IQueryHandler<GetSessionsPagedQuery, Result<(int, IEnumerable<SessionBasic>)>>
{
    public async Task<Result<(int, IEnumerable<SessionBasic>)>> HandleAsync(
        GetSessionsPagedQuery query,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
    }
}
