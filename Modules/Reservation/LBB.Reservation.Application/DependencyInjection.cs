using LBB.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace LBB.Reservation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
