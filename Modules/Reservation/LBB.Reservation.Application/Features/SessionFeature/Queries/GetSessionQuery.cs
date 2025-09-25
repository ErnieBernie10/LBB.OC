using FluentResults;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Dtos;
using LBB.Reservation.Domain;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Application.Features.SessionFeature.Queries;

public class GetSessionQuery : IQuery<Result<GetSessionResponseDto>>
{
    public int Id { get; set; }
}

public class GetSessionQueryHandler(LbbDbContext context)
    : IQueryHandler<GetSessionQuery, Result<GetSessionResponseDto>>
{
    public async Task<Result<GetSessionResponseDto>> HandleAsync(
        GetSessionQuery query,
        CancellationToken cancellationToken = default
    )
    {
        var session = await context
            .Sessions.Include(s => s.Reservations)
            .Select(s => new GetSessionResponseDto()
            {
                Id = s.Id,
                AttendeeCount = s.Reservations.Sum(r => r.AttendeeCount),
                Capacity = s.Capacity,
                Description = s.Description,
                Start = s.Start,
                End = s.End,
                Location = s.Location,
                Title = s.Title,
                Type = (Enums.SessionType)s.Type,
                CancelledOn = s.CancelledOn,
                Reservations = s.Reservations.Select(s => new GetReservationsResponseDto()
                {
                    Email = s.Email,
                    Firstname = s.Firstname,
                    Lastname = s.Lastname,
                    Phone = s.Phone,
                    Reference = s.Reference,
                    AttendeeCount = s.AttendeeCount,
                }),
            })
            .FirstOrDefaultAsync(s => s.Id == query.Id, cancellationToken: cancellationToken);

        if (session == null)
            return Result.Fail(new NotFoundError("Session not found"));

        return Result.Ok(session);
    }
}
