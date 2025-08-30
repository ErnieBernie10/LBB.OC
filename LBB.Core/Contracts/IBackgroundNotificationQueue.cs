using System.Windows.Input;
using LBB.Core.Mediator;

namespace LBB.Core.Contracts;

public interface IBackgroundNotificationQueue
{
    void Enqueue<TNotification>(TNotification command)
        where TNotification : INotification;

    ValueTask<INotification?> DequeueAsync(CancellationToken cancellationToken);
}
