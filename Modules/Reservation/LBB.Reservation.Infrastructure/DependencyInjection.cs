using LBB.Core;
using LBB.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace LBB.Reservation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWorkHandler, UnitOfWorkHandler>();
        return services;
    }
}
