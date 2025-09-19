using FluentValidation;
using LBB.Reservation.Application.Features.SessionFeature.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LBB.Reservation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(CreateSessionCommandValidator).Assembly);
        return services;
    }
}
