using LBB.Core.Contracts;
using LBB.Core.Mediator;
using OrchardCore.BackgroundTasks;

namespace LBB.OC.Reservation;

public class BackgroundNotificationHandler : IBackgroundNotificationQueue, IBackgroundTask
{
    private readonly Queue<INotification> _notifications = [];
    private const int MaxRetryAttempts = 3;
    private static readonly TimeSpan BaseDelay = TimeSpan.FromMilliseconds(250);

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

    public async Task DoWorkAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken
    )
    {
        var mediator = new Mediator(serviceProvider);

        var snapshot = new List<INotification>(_notifications.Count);
        while (_notifications.Count > 0)
        {
            snapshot.Add(_notifications.Dequeue());
        }

        // Retry with exponential backoff per notification; re-enqueue on final failure
        foreach (var n in snapshot)
        {
            var attempt = 0;
            Exception? lastError = null;

            while (attempt < MaxRetryAttempts && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await mediator.PublishAsync(n, cancellationToken);
                    lastError = null;
                    break;
                }
                catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
                {
                    lastError = ex;
                    attempt++;

                    if (attempt >= MaxRetryAttempts)
                    {
                        // Put back to the queue to try again on the next run
                        _notifications.Enqueue(n);
                        break;
                    }

                    var delay = TimeSpan.FromMilliseconds(
                        BaseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1)
                    );
                    try
                    {
                        await Task.Delay(delay, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // If cancelled during delay, re-enqueue and exit
                        _notifications.Enqueue(n);
                        return;
                    }
                }
            }

            if (lastError is not null && cancellationToken.IsCancellationRequested)
            {
                throw new ApplicationException(
                    "Could not publish notification "
                        + n.GetType().Name
                        + " after "
                        + MaxRetryAttempts
                        + " attempts.",
                    lastError
                );
            }
        }
    }
}
