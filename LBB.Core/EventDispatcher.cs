using LBB.Core.Contracts;
using LBB.Core.Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LBB.Core;

[Flags]
public enum DispatchTarget
{
    Outbox = 0,
    RealtimeHub = 1,
}

public class EventDispatcherService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<EventDispatcherService> _logger;
    private readonly List<Dispatcher> _dispatchers = [];

    public EventDispatcherService(IServiceProvider provider, ILogger<EventDispatcherService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public Dispatcher To(DispatchTarget target)
    {
        var dispatcher = new Dispatcher(target);
        _dispatchers.Add(dispatcher);
        return dispatcher;
    }

    public Task DispatchAsync()
    {
        var tasks = _dispatchers.SelectMany(
            (dispatcher) =>
            {
                var notifications = dispatcher.Notifications;
                return notifications.SelectMany(
                    (n) =>
                    {
                        var (identifier, notification) = n;
                        if (notification == null)
                            return [];
                        var t = new List<Task>(Enum.GetValues<DispatchTarget>().Length);
                        if (dispatcher.Target.HasFlag(DispatchTarget.RealtimeHub))
                        {
                            t.Add(DispatchRealtime(notification, identifier));
                        }

                        if (dispatcher.Target.HasFlag(DispatchTarget.Outbox))
                        {
                            t.Add(DispatchOutbox(notification, identifier));
                        }
                        return t;
                    }
                );
            }
        );
        return Task.WhenAll(tasks);
    }

    private async Task DispatchRealtime<T>(INotification notification, T? identifier)
    {
        var hub = _provider.GetService<IHubContext<RealtimeHub, IEventClient>>();
        if (hub == null)
            throw new InvalidOperationException("No hub context found");

        var topic = notification.GetType().Name;
        await hub.Clients.Group(topic).EventReceived(topic, notification);

        if (identifier != null)
            topic += $".{identifier}";
        await hub.Clients.Group(topic).EventReceived(topic, notification);
        _logger.LogInformation("Event dispatched to RealtimeHub topic: {Topic}", topic);
    }

    private async Task DispatchOutbox<T>(INotification notification, T? identifier)
    {
        var outbox = _provider.GetService<IOutboxService>();
        if (outbox == null)
            throw new InvalidOperationException("No outbox service found");

        await outbox.PublishAsync("", identifier?.ToString() ?? "", notification);
        _logger.LogInformation("Event dispatched to Outbox");
    }

    public class Dispatcher
    {
        private readonly List<(object?, INotification?)> _notifications = [];
        public IReadOnlyCollection<(object?, INotification?)> Notifications => _notifications;
        public DispatchTarget Target { get; }

        public Dispatcher(DispatchTarget target)
        {
            Target = target;
        }

        public Dispatcher QueueMessage(INotification notification, object? identifier = null)
        {
            _notifications.Add((identifier, notification));
            return this;
        }
    }
}
