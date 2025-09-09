using FluentResults;
using LBB.Core.Errors;
using LBB.Core.Mediator;
using LBB.OC.Reservation.Extensions;
using LBB.Reservation.Application.Features.SessionFeature.Commands;
using LBB.Reservation.Application.Features.SessionFeature.Dtos;
using LBB.Reservation.Application.Features.SessionFeature.Queries;
using LBB.Reservation.Infrastructure;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OrchardCore;

namespace LBB.OC.Reservation.Controllers;

[Route(Constants.ModuleBasePath + "/sessions")]
[ApiController]
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

    [HttpPost]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionCommand command)
    {
        var session = await mediator.SendCommandAsync<CreateSessionCommand, Result<int>>(command);
        if (session.IsFailed)
        {
            if (session.HasError<ValidationError>())
                return BadRequest(session.MapValidationErrorsToProblemDetails());
            if (session.HasError<NotFoundError>())
                return NotFound(session.Errors);
        }
        return Ok(session.Value);
    }

    [HttpPost("{sessionId:int}/reservations")]
    public async Task<IActionResult> AddReservation(int sessionId, [FromBody] AddReservationCommand command)
    {
        command.SessionId = sessionId;
        var session = await mediator.SendCommandAsync<AddReservationCommand, Result<int>>(command);
        if (session.IsFailed)
            return BadRequest(session.Errors);
        return Ok(session.Value);
    }
}