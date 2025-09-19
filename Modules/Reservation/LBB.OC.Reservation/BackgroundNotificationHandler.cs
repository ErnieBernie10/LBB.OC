using LBB.Core.Contracts;
using LBB.Core.Mediator;
using OrchardCore.BackgroundTasks;

namespace LBB.OC.Reservation;

public class BackgroundNotificationHandler : IBackgroundNotificationQueue, IBackgroundTask
{
    private readonly Queue<INotification> _notifications = [];

    public void Enqueue<TNotification>(TNotification command)
        where TNotification : INotification
    {
        _notifications.Enqueue(command);
    }

    public ValueTask<INotification?> DequeueAsync(CancellationToken cancellationToken)
    {
        var n = _notifications.Dequeue();
        return new ValueTask<INotification?>(n);
    }

    public Task DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var mediator = new Mediator(serviceProvider);

        var snapshot = new List<INotification>(_notifications.Count);
        // TODO: If publish fails add retry.
        while (_notifications.Count > 0)
        {
            snapshot.Add(_notifications.Dequeue());
        }

        var tasks = snapshot.Select(n => mediator.PublishAsync(n, CancellationToken.None));
        return Task.WhenAll(tasks);
    }
}
