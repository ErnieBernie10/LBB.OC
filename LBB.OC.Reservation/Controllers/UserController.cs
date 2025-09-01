using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LBB.OC.Reservation.Controllers;

public class UserDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
}

[ApiController]
[Route(Constants.ModuleBasePath + "/users")]
public class UserController : ControllerBase
{
    [HttpGet("current")]
    public IActionResult Index()
    {
        var user = User;
        return Ok(new UserDto()
        {
            UserName = user.Identity?.Name,
            Email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
        });
    }
}