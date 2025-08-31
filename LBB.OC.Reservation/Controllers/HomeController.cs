using System.ComponentModel.DataAnnotations;
using LBB.OC.Reservation.ViewModels;
using LBB.OC.Reservation.ViewModels.Session;
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
        return View(new IndexVM(sessions, new CreateSessionVM(new CreateSessionVM.CreateSessionForm())));
    }

    [HttpGet]
    public IActionResult CreateSession()
    {
        return PartialView("Session/_CreateSession", new CreateSessionVM(new CreateSessionVM.CreateSessionForm()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSession([Bind(Prefix = "Form")] CreateSessionVM.CreateSessionForm form)
    {
        if (!ModelState.IsValid)
        {
            // Return the form with validation messages
            return PartialView("Session/_CreateSession", new CreateSessionVM(form));
        }
        //
        // var entity = new LBB.Reservation.Infrastructure.DataModels.Session
        // {
        //     Title = form.Title,
        //     Description = form.Description,
        //     Start = DateTime.UtcNow,
        //     End = DateTime.UtcNow.AddHours(1),
        //     UserId = "system"
        // };
        // context.Sessions.Add(entity);
        // await context.SaveChangesAsync();

        // Return a simple success modal content which will replace the modal's inner HTML
        return PartialView("Session/_CreateSessionSuccess");
    }
}