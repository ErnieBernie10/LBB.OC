using LBB.Core.Mediator;

namespace LBB.Core.Contracts;

public interface IOutboxService
{
    Task PublishAsync<T>(
        string aggregateType,
        string aggregateId,
        string type,
        T payload,
        CancellationToken cancellationToken = default
    )
        where T : INotification;

    Task PublishAsync<T>(
        string aggregateType,
        string aggregateId,
        T payload,
        CancellationToken cancellationToken = default
    )
        where T : INotification;
}
