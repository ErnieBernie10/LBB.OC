using LBB.Core;
using LBB.Core.Contracts;
using LBB.Reservation.Domain.Aggregates.Session;
using Microsoft.Extensions.DependencyInjection;

namespace LBB.Reservation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWorkHandler, UnitOfWorkHandler>();
        services.AddScoped<IAggregateStore<Session, int>, SessionStore>();
        return services;
    }
}
