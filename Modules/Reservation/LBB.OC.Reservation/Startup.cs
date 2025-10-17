using LBB.Core;
using LBB.Core.Contracts;
using LBB.Core.Mediator.PipelineBehaviors;
using LBB.OC.Outbox;
using LBB.OC.Reservation.Migrations;
using LBB.Reservation.Application;
using LBB.Reservation.Infrastructure;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrchardCore.BackgroundTasks;
using OrchardCore.Data.Migration;
using OrchardCore.Email;
using OrchardCore.Environment.Shell;
using DependencyInjection = LBB.Reservation.Infrastructure.DependencyInjection;
using StartupBase = OrchardCore.Modules.StartupBase;

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

        services
            .AddCore(
                typeof(Startup).Assembly,
                typeof(ApplicationAssembly).Assembly,
                typeof(DependencyInjection).Assembly
            )
            .AddPipelineBehavior(typeof(LoggingBehavior<,>))
            .AddApplication()
            .AddInfrastructure();

        services.AddDbContext<LbbDbContext>(
            (serviceProvider, options) =>
            {
                var shellSettings = serviceProvider.GetRequiredService<ShellSettings>();
                var connectionString =
                    $"Data Source=App_Data/Sites/{shellSettings.Name}/{shellSettings["DatabaseName"]}";
                options.UseSqlite(connectionString);
            }
        );
        services.ConfigureReservationModuleAuthorization();
        services.AddSwaggerGen();
        services.AddHttpContextAccessor();
        services.AddSignalR();
        services.AddScoped<IOutboxService, EfOutboxService<LbbDbContext>>();

        services.AddSingleton<ISpaProvider, SpaProvider>();
    }

    public override void Configure(
        IApplicationBuilder builder,
        IEndpointRouteBuilder routes,
        IServiceProvider serviceProvider
    )
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
            defaults: new { controller = "Home", action = "Index" }
        );

        routes.MapHub<RealtimeHub>($"{Constants.ModuleBasePath}/realtime");
    }
}
