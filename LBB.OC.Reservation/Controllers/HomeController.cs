using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Environment.Extensions;

namespace LBB.OC.Reservation.Controllers
{
    [Route("reservation")]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public HomeController(IWebHostEnvironment   env)
        {
            _env = env;
        }

        // Serves /reservation and any client-side route under it (e.g., /reservation/users/123)
        [HttpGet("")]
        [HttpGet("{*slug}")]
        public IActionResult Index()
        {
            var root = _env.ContentRootPath;
            var moduleWebRoot = Path.Combine(root, "../LBB.OC.Reservation/wwwroot");
            var index = Path.Combine(moduleWebRoot, "index.html");
            var fileInfo = new FileInfo(index);
            if (!fileInfo.Exists)
            {
                return NotFound("SPA index.html not found at module path: LBB.OC.Reservation/wwwroot/index.html");
            }
            return PhysicalFile(index, "text/html");
        }
    }
}