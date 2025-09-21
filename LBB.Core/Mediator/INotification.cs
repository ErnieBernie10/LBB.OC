namespace LBB.Core.Mediator;

public interface INotification { }

public interface IOutOfProcessNotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task HandleAsync(TNotification command, CancellationToken cancellationToken = default);
}

public interface IInProcessNotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task HandleAsync(TNotification command, CancellationToken cancellationToken = default);
}
