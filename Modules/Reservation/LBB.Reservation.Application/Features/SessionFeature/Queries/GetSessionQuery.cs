using FluentResults;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session;

namespace LBB.Reservation.Application.Features.SessionFeature.Queries;

public class GetSessionQuery : IQuery<Result<Session>>
{
    public int Id { get; set; }
}

public class GetSessionQueryHandler : IQueryHandler<GetSessionQuery, Result<Session>>
{
    public async Task<Result<Session>> HandleAsync(
        GetSessionQuery query,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotImplementedException();
        // var session = await repo.Get(query.Id);
        // if (session == null)
        //     return new NotFoundError($"Session with id {query.Id} not found");
        //
        // return Result.Ok(session);
    }
}
