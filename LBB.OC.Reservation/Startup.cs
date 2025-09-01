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

        // Register Alpine.js resources
        services.Configure<ResourceManagementOptions>(options =>
        {
            var manifest = new ResourceManifest();

            manifest.DefineScript("alpinejs")
                .SetUrl("https://cdn.jsdelivr.net/npm/alpinejs@3.14.1/dist/cdn.min.js")
                .SetCdn("https://cdn.jsdelivr.net/npm/alpinejs@3.14.1/dist/cdn.min.js")
                .SetVersion("3.14.1")
                .SetAttribute("defer", "defer");

            manifest.DefineScript("alpine-ajax")
                .SetUrl("https://cdn.jsdelivr.net/npm/@imacrayon/alpine-ajax@0.12.4/dist/cdn.min.js")
                .SetCdn("https://cdn.jsdelivr.net/npm/@imacrayon/alpine-ajax@0.12.4/dist/cdn.min.js")
                .SetVersion("0.12.4")
                .SetAttribute("defer", "defer");

            options.ResourceManifests.Add(manifest);
        });
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
                string.Equals(context.Request.Path.Value, "/reservation", StringComparison.OrdinalIgnoreCase))
            {
                var qs = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;
                context.Response.Redirect($"{context.Request.PathBase}/reservation/{qs}", permanent: false);
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
            pattern: "reservation/{**slug}",
            defaults: new { controller = "Home", action = "Index" });
    }
}