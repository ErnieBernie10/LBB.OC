namespace LBB.Core.Contracts;

public interface IOutboxService
{
    Task PublishAsync(
        string aggregateType,
        string aggregateId,
        string type,
        object payload,
        CancellationToken cancellationToken = default
    );
}
