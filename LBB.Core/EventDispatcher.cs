using LBB.Core.Contracts;
using LBB.Core.Mediator;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

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

    public EventDispatcherService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Dispatcher To(DispatchTarget target)
    {
        return new Dispatcher(_provider, target);
    }

    public class Dispatcher
    {
        private readonly IServiceProvider _provider;
        private readonly DispatchTarget _target;

        public Dispatcher(IServiceProvider provider, DispatchTarget target)
        {
            _provider = provider;
            _target = target;
        }

        public Task DispatchAsync(INotification notification)
        {
            var tasks = new List<Task>();
            if (_target.HasFlag(DispatchTarget.RealtimeHub))
            {
                tasks.Add(DispatchRealtime(notification));
            }
            if (_target.HasFlag(DispatchTarget.Outbox))
            {
                tasks.Add(DispatchOutbox(notification));
            }
            return Task.WhenAll(tasks);
        }

        private async Task DispatchRealtime(INotification notification)
        {
            var hub = _provider.GetService<IHubContext<RealtimeHub, IEventClient>>();
            if (hub == null)
                throw new InvalidOperationException("No hub context found");

            await hub.Clients.All.EventReceived(notification.GetType().Name, notification);
        }

        private async Task DispatchOutbox(INotification notification)
        {
            var outbox = _provider.GetService<IOutboxService>();
            if (outbox == null)
                throw new InvalidOperationException("No outbox service found");

            await outbox.PublishAsync("", "", notification);
        }
    }
}
