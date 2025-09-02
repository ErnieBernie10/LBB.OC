using System.Security.Claims;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LBB.OC.Reservation.Controllers;

public class UserDto
{
    [JsonPropertyName("username")]
    public string? UserName { get; set; }
    [JsonPropertyName("email")]
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