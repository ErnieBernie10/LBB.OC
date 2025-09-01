using System;
using System.IO;
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
using Microsoft.Extensions.FileProviders;

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

    public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes,
        IServiceProvider serviceProvider)
    {
        builder.UseAntiforgery();

        // Serve SPA from a dedicated request path to avoid collisions with Orchard routes,
        // and to behave consistently in sub-tenants with URL prefixes.
        var requestPath = "/reservation";

        // Default files (serves index.html at /reservation)
        builder.UseDefaultFiles(new DefaultFilesOptions
        {
            RequestPath = requestPath,
            FileProvider = new SpaFileProvider(serviceProvider.GetRequiredService<IApplicationContext>()),
            // DefaultFileNames already includes "index.html", but we rely on our provider anyway.
        });

        // Static files for SPA assets under /reservation (js/css/img, etc.)
        builder.UseStaticFiles(new StaticFileOptions
        {
            RequestPath = requestPath,
            FileProvider = new SpaFileProvider(serviceProvider.GetRequiredService<IApplicationContext>())
        });

        // Optional: client-side routing fallback for deep links under /reservation/**
        // routes.MapAreaControllerRoute(
        //     name: "ReservationSpa",
        //     areaName: "LBB.OC.Reservation",
        //     pattern: "reservation/{**slug}",
        //     defaults: new { controller = "Home", action = "Index" });
    }
}