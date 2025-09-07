using System.Globalization;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet("{**slug}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Constants.Policies.ManageReservations)]
        public IActionResult Index(string? slug)
        {
            // If a file extension is present (e.g., .js, .css, .png), redirect to the root so StaticFiles serves it.
            if (!string.IsNullOrEmpty(slug) && Path.HasExtension(slug))
            {
                var target = "/" + slug.TrimStart('/');
                return LocalRedirect(target);
            }

            // Serve from memory to avoid per-request I/O.
            return File(_spaIndex.GetBytes(slug?.Split("/").FirstOrDefault() ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)!, "text/html; charset=utf-8");
        }
    }
}