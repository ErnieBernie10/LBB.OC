using System;
using System.IO;
using System.Reflection;
using LBB.Core;
using LBB.OC.Reservation.Migrations;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.Environment.Shell;
using OrchardCore.ResourceManagement;
using YesSql;
using Microsoft.Extensions.FileProviders;
using StartupBase = OrchardCore.Modules.StartupBase;
using Microsoft.AspNetCore.Authorization;
using OrchardCore;

namespace LBB.OC.Reservation;

public class Startup : StartupBase
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

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Constants.Policies.ManageReservations, policy =>
                policy.RequireRole(
                    OrchardCoreConstants.Roles.Administrator,
                    Constants.Roles.ReservationManager));
        });

        // Cache SPA index.html in memory once (singleton)
        services.AddSingleton<ISpaProvider, SpaProvider>();
    }

    private string GetModuleWebRoot(IWebHostEnvironment? env)
    {
        if (env == null)
            return string.Empty;

        var ocRoot = env.ContentRootPath;
        return Path.Combine(ocRoot, "../LBB.OC.Reservation/wwwroot");
    }

    public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes,
        IServiceProvider serviceProvider)
    {
        builder.UseAntiforgery();

        // Redirect /reservation -> /reservation/ (tenant-aware via PathBase)
        builder.Use(async (context, next) =>
        {
            if (HttpMethods.IsGet(context.Request.Method) &&
                string.Equals(context.Request.Path.Value, $"/{Constants.ModuleBasePath}", StringComparison.OrdinalIgnoreCase))
            {
                var qs = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
                context.Response.Redirect($"{context.Request.PathBase}/{Constants.ModuleBasePath}/{qs}", permanent: false);
                return;
            }

            await next();
        });

        var env = serviceProvider.GetService<IWebHostEnvironment>();

        builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(GetModuleWebRoot(env))
        });

        // Explicit SPA fallback route for both default and prefixed tenants.
        routes.MapAreaControllerRoute(
            name: "ReservationSpa",
            areaName: "LBB.OC.Reservation",
            pattern: Constants.ModuleBasePath + "/{**slug}",
            defaults: new { controller = "Home", action = "Index" });
    }
}