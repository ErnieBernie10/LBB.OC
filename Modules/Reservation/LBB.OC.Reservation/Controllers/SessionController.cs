using LBB.Core.Mediator;
using LBB.Reservation.Application.Features.SessionFeature.Dtos;
using LBB.Reservation.Application.Features.SessionFeature.Queries;
using LBB.Reservation.Infrastructure;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore;

namespace LBB.OC.Reservation.Controllers;

[Route(Constants.ModuleBasePath + "/sessions")]
public class SessionController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<IActionResult> GetSessions(DateTimeOffset? from, DateTimeOffset? to)
    {
        var sessions = await mediator.SendQueryAsync<GetSessionsQuery, IEnumerable<GetSessionsResponseDto>>(new GetSessionsQuery()
        {
            Start = from,
            End = to
        });

        return Ok(sessions);
    }
}