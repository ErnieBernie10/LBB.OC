using Microsoft.AspNetCore.Mvc;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders;

namespace LBB.OC.Reservation.Controllers;

public sealed class HomeController(ShellSettings settings) : Controller
{
    public ActionResult Index()
    {
        var connectionString = settings.ShellConfiguration["ConnectionString"];
        return View();
    }
}