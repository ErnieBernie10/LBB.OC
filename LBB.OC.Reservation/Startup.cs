using System.Reflection;
using LBB.Core;
using LBB.OC.Reservation.Migrations;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;
using YesSql;

namespace LBB.OC.Reservation;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDataMigration<ReservationMigrations>();

        services.AddCore(typeof(Startup).Assembly);

        services.AddDbContext<LbbDbContext>((serviceProvider, options) =>
        {
            var shellSettings = serviceProvider.GetRequiredService<ShellSettings>();
            var connectionString = $"Data Source=App_Data/Sites/{shellSettings.Name}/{shellSettings["DatabaseName"]}";
            options.UseSqlite(connectionString);
        });

    }

    public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes,
        IServiceProvider serviceProvider)
    {

        builder.UseStaticFiles();
        builder.UseAntiforgery();

        routes.MapAreaControllerRoute(
            name: "Reservation_Home",
            areaName: "LBB.OC.Reservation",
            pattern: "Home/Index",
            defaults: new { controller = "Home", action = "Index" }
        );
    }
}