using System.Reflection;
using LBB.Core.Contracts;
using LBB.Core.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace LBB.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(
        this IServiceCollection services,
        params Assembly[] assemblies
    )
    {
        services.AddScoped<IMediator, Mediator.Mediator>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        foreach (var assembly in assemblies)
        {
            RegisterHandlers(services, assembly, typeof(ICommandHandler<,>));
            RegisterHandlers(services, assembly, typeof(IQueryHandler<,>));
            RegisterHandlers(services, assembly, typeof(INotificationHandler<>));
        }

        return services;
    }

    private static void RegisterHandlers(
        IServiceCollection services,
        Assembly assembly,
        Type openGenericType
    )
    {
        var handlerTypes = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Where(t =>
                t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
            )
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var interfaces = handlerType
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType);

            foreach (var iface in interfaces)
            {
                services.AddScoped(iface, handlerType);
            }
        }
    }
}
