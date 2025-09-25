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
        if (session.IsFailed)
        {
            if (session.HasError<NotFoundError>())
                return NotFound(session.Errors);
            return BadRequest(session.Errors);
        }
        return Ok(session.Value);
    }

    [HttpPost]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<ActionResult<int>> CreateSession([FromBody] CreateSessionCommand command)
    {
        var session = await mediator.SendCommandAsync<CreateSessionCommand, Result<int>>(command);
        if (session.IsFailed)
        {
            if (session.HasError<ValidationError>())
                return BadRequest(session.MapValidationErrorsToProblemDetails());
            if (session.HasError<NotFoundError>())
                return NotFound(session.Errors);
            return BadRequest(session.Errors);
        }
        return Ok(session.Value);
    }

    [HttpPatch("{id:int}/cancel")]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<ActionResult<int>> CancelSession([FromRoute] int id)
    {
        var command = new CancelSessionCommand() { SessionId = id };

        var result = await mediator.SendCommandAsync<CancelSessionCommand, Result>(command);
        if (result.IsFailed)
        {
            if (result.HasError<ValidationError>())
                return BadRequest(result.MapValidationErrorsToProblemDetails());
            if (result.HasError<NotFoundError>())
                return NotFound(result.Errors);
            return BadRequest(result.Errors);
        }
        return Ok();
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
        if (session.IsFailed)
        {
            if (session.HasError<ValidationError>())
                return BadRequest(session.MapValidationErrorsToProblemDetails());
            if (session.HasError<NotFoundError>())
                return NotFound(session.Errors);
            return BadRequest(session.Errors);
        }
        return Ok();
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
        if (reservations.IsFailed)
            return BadRequest(reservations.Errors);
        return Ok(reservations.Value);
    }

    [HttpPost("{sessionId:int}/reservations")]
    public async Task<ActionResult<int>> AddReservation(
        int sessionId,
        [FromBody] AddReservationCommand command
    )
    {
        command.SessionId = sessionId;
        var session = await mediator.SendCommandAsync<AddReservationCommand, Result<int>>(command);
        if (session.IsFailed)
        {
            if (session.HasError<ValidationError>())
                return BadRequest(session.MapValidationErrorsToProblemDetails());
            if (session.HasError<NotFoundError>())
                return NotFound(session.Errors);

            return BadRequest(session.Errors);
        }
        return Ok(session.Value);
    }

    [HttpDelete("{sessionId:int}/reservations/{reservationId:int}")]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<ActionResult<int>> RemoveReservation(int sessionId, int reservationId)
    {
        var command = new CancelReservationCommand();

        var result = await mediator.SendCommandAsync<CancelReservationCommand, Result>(command);
        if (result.IsFailed)
        {
            if (result.HasError<ValidationError>())
                return BadRequest(result.MapValidationErrorsToProblemDetails());
            if (result.HasError<NotFoundError>())
                return NotFound(result.Errors);
            return BadRequest(result.Errors);
        }
        return Ok();
    }

    [HttpDelete("{sessionId:int}")]
    [Authorize(Constants.Policies.ManageReservations)]
    public async Task<ActionResult<int>> RemoveSession(int sessionId)
    {
        var command = new DeleteSessionCommand() { SessionId = sessionId };

        var result = await mediator.SendCommandAsync<DeleteSessionCommand, Result>(command);
        if (result.IsFailed)
        {
            if (result.HasError<ValidationError>())
                return BadRequest(result.MapValidationErrorsToProblemDetails());
            if (result.HasError<NotFoundError>())
                return NotFound(result.Errors);
            return BadRequest(result.Errors);
        }
        return Ok();
    }
}
