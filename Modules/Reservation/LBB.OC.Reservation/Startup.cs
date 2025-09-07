using System;
using System.IO;
using System.Reflection;
using LBB.Core;
using LBB.OC.Reservation.Migrations;
using LBB.Reservation.Application;
using LBB.Reservation.Infrastructure;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using OrchardCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace LBB.OC.Reservation;

public class Startup : StartupBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public Startup(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDataMigration<ReservationMigrations>();

        services.AddCore(typeof(Startup).Assembly, typeof(ApplicationAssembly).Assembly);
        services.AddApplication();

        services.AddDbContext<LbbDbContext>((serviceProvider, options) =>
        {
            var shellSettings = serviceProvider.GetRequiredService<ShellSettings>();
            var connectionString = $"Data Source=App_Data/Sites/{shellSettings.Name}/{shellSettings["DatabaseName"]}";
            options.UseSqlite(connectionString);
        });
        services.AddScoped<IUnitOfWorkHandler, UnitOfWorkHandler>();

        services.ConfigureReservationModuleAuthorization();
        services.AddSwaggerGen();
        services.AddFluentValidationAutoValidation();
        services.AddHttpContextAccessor();

        services.AddSingleton<ISpaProvider, SpaProvider>();

    }

    public override void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes,
        IServiceProvider serviceProvider)
    {
        builder.UseAntiforgery();

        builder.UseReservationMiddleware(serviceProvider);
        if (_webHostEnvironment.IsDevelopment())
        {
            builder.UseSwagger();
            builder.UseSwaggerUI();
        }

        // Explicit SPA fallback route for both default and prefixed tenants.
        routes.MapAreaControllerRoute(
            name: "ReservationSpa",
            areaName: Constants.ModuleName,
            pattern: Constants.ModuleBasePath + "/{**slug}",
            defaults: new { controller = "Home", action = "Index" });
    }
}