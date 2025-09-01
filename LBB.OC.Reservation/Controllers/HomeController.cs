using Microsoft.AspNetCore.Mvc;
using OrchardCore.Environment.Shell;

namespace LBB.OC.Reservation.Controllers
{
    [Route(Constants.ModuleBasePath)]
    public class HomeController : ControllerBase
    {
        private readonly ISpaProvider _spaIndex;
        private readonly ShellSettings _shellSettings;

        public HomeController(ISpaProvider spaIndex, ShellSettings shellSettings)
        {
            _spaIndex = spaIndex;
            _shellSettings = shellSettings;
        }

        // GET /reservation
        // GET /reservation/{anything}
        // If "anything" looks like a static asset (has an extension), redirect to root ("/...") so it is served by static files.
        [HttpGet("")]
        [HttpGet("{*slug}")]
        public IActionResult Index(string? slug)
        {
            // If a file extension is present (e.g., .js, .css, .png), redirect to the root so StaticFiles serves it.
            if (!string.IsNullOrEmpty(slug) && Path.HasExtension(slug))
            {
                var target = "/" + slug.TrimStart('/');
                return LocalRedirect(target);
            }

            if (!_spaIndex.Exists || _spaIndex.Bytes is null)
            {
                return NotFound("SPA index.html not found at module path: LBB.OC.Reservation/wwwroot/index.html");
            }

            // Serve from memory to avoid per-request I/O.
            return File(_spaIndex.Bytes, "text/html; charset=utf-8");
        }
    }
}