using LBB.Core.Mediator;

namespace LBB.OC.Outbox;

public static class NotificationCache
{
    private static readonly Dictionary<string, Type> _cache;

    static NotificationCache()
    {
        // Scan all loaded assemblies for types implementing INotification (your domain events)
        _cache = AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(INotification).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(t => t.Name!, t => t);
    }

    public static Type? GetTypeByName(string name)
    {
        _cache.TryGetValue(name, out var type);
        return type;
    }
}
