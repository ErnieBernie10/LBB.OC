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

    public EventDispatcherService(IServiceProvider provider, ILogger<EventDispatcherService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public Dispatcher To(DispatchTarget target)
    {
        return new Dispatcher(_provider, target, _logger);
    }

    public class Dispatcher
    {
        private readonly ILogger<EventDispatcherService> _logger;
        private readonly IServiceProvider _provider;
        private readonly DispatchTarget _target;

        public Dispatcher(
            IServiceProvider provider,
            DispatchTarget target,
            ILogger<EventDispatcherService> logger
        )
        {
            _provider = provider;
            _target = target;
            _logger = logger;
        }

        public Task DispatchAsync(INotification notification)
        {
            return DispatchAsync<string>(notification, null);
        }

        public Task DispatchAsync<T>(INotification notification, T? identifier)
        {
            var tasks = new List<Task>();
            if (_target.HasFlag(DispatchTarget.RealtimeHub))
            {
                tasks.Add(DispatchRealtime(notification, identifier));
            }
            if (_target.HasFlag(DispatchTarget.Outbox))
            {
                tasks.Add(DispatchOutbox(notification));
            }
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

        private async Task DispatchOutbox(INotification notification)
        {
            var outbox = _provider.GetService<IOutboxService>();
            if (outbox == null)
                throw new InvalidOperationException("No outbox service found");

            await outbox.PublishAsync("", "", notification);
            _logger.LogInformation("Event dispatched to Outbox");
        }
    }
}
