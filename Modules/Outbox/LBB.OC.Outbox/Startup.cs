using LBB.OC.Outbox.Migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;

namespace LBB.OC.Outbox;

public sealed class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddDataMigration<OutboxMigrations>();
    }

    public override void Configure(
        IApplicationBuilder builder,
        IEndpointRouteBuilder routes,
        IServiceProvider serviceProvider
    ) { }
}
