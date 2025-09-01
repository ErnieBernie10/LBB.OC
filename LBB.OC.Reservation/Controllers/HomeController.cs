using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace LBB.OC.Reservation.Controllers
{
    [Route("reservation")]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // GET /reservation
        // GET /reservation/{anything}
        // If "anything" looks like a static asset (has an extension), redirect to root ("/...") so it is served by static files.
        [HttpGet("")]
        [HttpGet("{*slug}")]
        public IActionResult Index(string? slug)
        {
            // If a file extension is present (e.g., .js, .css, .png), redirect to the root so StaticFiles serves it.
            if (!string.IsNullOrEmpty(slug) && System.IO.Path.HasExtension("/" + slug))
            {
                var target = "/" + slug.TrimStart('/');
                return LocalRedirect(target);
            }

            var root = _env.ContentRootPath;
            var moduleWebRoot = System.IO.Path.Combine(root, "../LBB.OC.Reservation/wwwroot");
            var index = System.IO.Path.Combine(moduleWebRoot, "index.html");
            if (!System.IO.File.Exists(index))
            {
                return NotFound("SPA index.html not found at module path: LBB.OC.Reservation/wwwroot/index.html");
            }

            return PhysicalFile(index, "text/html; charset=utf-8");
        }
    }
}