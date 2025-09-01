using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore;

namespace LBB.OC.Reservation.Controllers;

[Route(Constants.ModuleBasePath + "/sessions")]
public class SessionController : ControllerBase
{
    [HttpGet]
    [Authorize(Constants.Policies.ManageReservations)]
    public IActionResult GetSessions()
    {
        return Ok();
    }
}