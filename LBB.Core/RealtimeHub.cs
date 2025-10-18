using System.Collections.Concurrent;
using LBB.Core.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace LBB.Core;

public class RealtimeHub : Hub<IEventClient>
{
    private static readonly ConcurrentDictionary<string, HashSet<string>> _subscriptions = new();

    // TODO : Add possibility to listen to a topic for a specific resource
    public async Task Subscribe(string topic)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, topic);
        _subscriptions.AddOrUpdate(
            topic,
            _ => new HashSet<string> { Context.ConnectionId },
            (_, set) =>
            {
                set.Add(Context.ConnectionId);
                return set;
            }
        );
    }

    public async Task Unsubscribe(string topic)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, topic);
        if (_subscriptions.TryGetValue(topic, out var set))
        {
            set.Remove(Context.ConnectionId);
            if (set.Count == 0)
                _subscriptions.TryRemove(topic, out _);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var kvp in _subscriptions)
        {
            kvp.Value.Remove(Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
