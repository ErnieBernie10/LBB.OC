using FluentResults;
using LBB.Core.Errors;
using LBB.OC.Reservation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LBB.OC.Reservation.Controllers;

public abstract class LbbApiControllerBase : ControllerBase
{
    public ActionResult HandleProblems(ResultBase result)
    {
        if (result.HasError<ValidationError>())
            return BadRequest(result.MapValidationErrorsToProblemDetails());
        if (result.HasError<NotFoundError>())
            return NotFound(result.Errors);
        return BadRequest(result.Errors);
    }
}
