using LBB.OC.Outbox.Context;
using LBB.OC.Outbox.Migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.BackgroundTasks;
using OrchardCore.Data.Migration;
using OrchardCore.Environment.Shell;
using OrchardCore.Modules;

namespace LBB.OC.Outbox;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDataMigration<OutboxMigrations>();
        services.AddDbContext<OutboxDbContext>(
            (serviceProvider, options) =>
            {
                var shellSettings = serviceProvider.GetRequiredService<ShellSettings>();
                var connectionString =
                    $"Data Source=App_Data/Sites/{shellSettings.Name}/{shellSettings["DatabaseName"]}";
                options.UseSqlite(connectionString);
            }
        );
        services.AddSingleton<IBackgroundTask, OutboxMessageRelay>();
    }

    public override void Configure(
        IApplicationBuilder builder,
        IEndpointRouteBuilder routes,
        IServiceProvider serviceProvider
    ) { }
}
