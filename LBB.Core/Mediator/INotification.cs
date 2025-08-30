namespace LBB.Core.Mediator;

public interface INotification { }

public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task HandleAsync(TNotification command, CancellationToken cancellationToken = default);
}
