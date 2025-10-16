using FluentResults;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Dtos;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Application.Features.SessionFeature.Queries;

public class GetReservationsBySessionQuery : IQuery<Result<IEnumerable<GetReservationsResponseDto>>>
{
    public int SessionId { get; set; }
}

public class GetReservationsBySessionQueryHandler(LbbDbContext context)
    : IQueryHandler<GetReservationsBySessionQuery, Result<IEnumerable<GetReservationsResponseDto>>>
{
    public async Task<Result<IEnumerable<GetReservationsResponseDto>>> HandleAsync(
        GetReservationsBySessionQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var reservations = await context
            .Reservations.Where(r => r.SessionId == query.SessionId)
            .Select(s => new GetReservationsResponseDto()
            {
                Id = s.Id,
                Email = s.Email,
                Firstname = s.Firstname,
                Lastname = s.Lastname,
                Phone = s.Phone,
                AttendeeCount = s.AttendeeCount,
                Reference = s.Reference,
                CancelledOn = s.CancelledOn,
            })
            .ToListAsync(cancellationToken: cancellationToken);

        return Result.Ok(reservations.AsEnumerable());
    }
}
