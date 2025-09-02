using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace LBB.OC.Reservation.Controllers;

[ApiController]
[Route(Constants.ModuleBasePath + "/csrf")]
public class CsrfController(IAntiforgery antiforgery) : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok(new { token = antiforgery.GetAndStoreTokens(HttpContext).RequestToken });
    }
}