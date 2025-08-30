using LBB.OC.Reservation.ViewModels;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrchardCore.Data;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders;
using YesSql;

namespace LBB.OC.Reservation.Controllers;

public sealed class HomeController(LbbDbContext context) : Controller
{
    public async Task<ActionResult> Index()
    {
        var sessions = await context.Sessions.ToListAsync();
        return View(new IndexVM(sessions));
    }
}