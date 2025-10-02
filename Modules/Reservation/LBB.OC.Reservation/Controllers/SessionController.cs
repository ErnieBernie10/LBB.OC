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
public class SessionController(IMediator mediator) : LbbApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetSessionsResponseDto>>> GetSessions(
        DateTimeOffset? from,
        DateTimeOffset? to
    )
    {
        var sessions = await mediator.SendQueryAsync<
            GetSessionsQuery,
            IEnumerable<GetSessionsResponseDto>
        >(new GetSessionsQuery() { Start = from, End = to });

        return Ok(sessions);
    }

    [HttpGet("{sessionId:int}")]
    public async Task<ActionResult<GetSessionResponseDto>> GetSession(int sessionId)
    {
        var query = new GetSessionQuery() { Id = sessionId };
        var session = await mediator.SendQueryAsync<GetSessionQuery, Result<GetSessionResponseDto>>(
            query
        );
        return session.IsFailed ? HandleProblems(session) : Ok(session.Value);
    }

    [HttpPost]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<ActionResult<int>> CreateSession([FromBody] CreateSessionCommand command)
    {
        var session = await mediator.SendCommandAsync<CreateSessionCommand, Result<int>>(command);
        return session.IsFailed ? HandleProblems(session) : Ok(session.Value);
    }

    [HttpPatch("{id:int}/cancel")]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<ActionResult<int>> CancelSession([FromRoute] int id)
    {
        var command = new CancelSessionCommand() { SessionId = id };

        var result = await mediator.SendCommandAsync<CancelSessionCommand, Result>(command);
        return result.IsFailed ? HandleProblems(result) : Ok();
    }

    [HttpPatch("{id:int}")]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<IActionResult> UpdateSessionInfo(
        [FromBody] UpdateSessionInfoCommand command,
        [FromRoute] int id
    )
    {
        command.SessionId = id;
        var session = await mediator.SendCommandAsync<UpdateSessionInfoCommand, Result>(command);
        return session.IsFailed ? HandleProblems(session) : Ok();
    }

    [HttpGet("{sessionId:int}/reservations")]
    public async Task<
        ActionResult<IEnumerable<GetReservationsResponseDto>>
    > GetReservationsBySession(int sessionId)
    {
        var query = new GetReservationsBySessionQuery() { SessionId = sessionId };
        var reservations = await mediator.SendQueryAsync<
            GetReservationsBySessionQuery,
            Result<IEnumerable<GetReservationsResponseDto>>
        >(query);
        return reservations.IsFailed ? HandleProblems(reservations) : Ok(reservations.Value);
    }

    [HttpPost("{sessionId:int}/reservations")]
    public async Task<ActionResult<int>> AddReservation(
        int sessionId,
        [FromBody] AddReservationCommand command
    )
    {
        command.SessionId = sessionId;
        var session = await mediator.SendCommandAsync<AddReservationCommand, Result<int>>(command);
        return session.IsFailed ? HandleProblems(session) : Ok(session.Value);
    }

    [HttpDelete("{sessionId:int}/reservations/{reservationId:int}")]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<ActionResult<int>> RemoveReservation(int sessionId, int reservationId)
    {
        var command = new CancelReservationCommand
        {
            SessionId = sessionId,
            ReservationId = reservationId,
        };

        var result = await mediator.SendCommandAsync<CancelReservationCommand, Result>(command);
        return result.IsFailed ? HandleProblems(result) : Ok();
    }

    [HttpDelete("{sessionId:int}")]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<ActionResult<int>> RemoveSession(int sessionId)
    {
        var command = new DeleteSessionCommand() { SessionId = sessionId };

        var result = await mediator.SendCommandAsync<DeleteSessionCommand, Result>(command);
        return result.IsFailed ? HandleProblems(result) : Ok();
    }

    [HttpPatch("{sessionId:int}/reservations/{reservationId:int}")]
    public async Task<ActionResult> UpdateReservation(
        int sessionId,
        int reservationId,
        [FromBody] UpdateReservationCommand command
    )
    {
        command.SessionId = sessionId;
        command.ReservationId = reservationId;
        var session = await mediator.SendCommandAsync<UpdateReservationCommand, Result>(command);
        return session.IsFailed ? HandleProblems(session) : Ok();
    }
}
