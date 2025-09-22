using LBB.Core.Contracts;
using LBB.Core.Mediator;

namespace LBB.OC.Outbox;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public sealed class EfOutboxService<TDbContext> : IOutboxService
    where TDbContext : DbContext
{
    private readonly TDbContext _db;

    public EfOutboxService(TDbContext db)
    {
        _db = db;
    }

    public async Task PublishAsync<T>(
        string aggregateType,
        string aggregateId,
        string type,
        T payload,
        CancellationToken cancellationToken = default
    )
        where T : INotification
    {
        var outboxEvent = new DataModels.Outbox()
        {
            AggregateType = aggregateType,
            AggregateId = aggregateId,
            Type = type,
            Payload = JsonSerializer.Serialize(payload),
            CreatedAt = DateTime.UtcNow,
            Module = "", // TODO
        };

        await _db.Set<DataModels.Outbox>().AddAsync(outboxEvent, cancellationToken);
    }

    public async Task PublishAsync<T>(
        string aggregateType,
        string aggregateId,
        T payload,
        CancellationToken cancellationToken = default
    )
        where T : INotification
    {
        var outboxEvent = new DataModels.Outbox()
        {
            AggregateType = aggregateType,
            AggregateId = aggregateId,
            Type = payload.GetType().Name,
            Payload = JsonSerializer.Serialize(payload),
            CreatedAt = DateTime.UtcNow,
            Module = "", // TODO
        };

        await _db.Set<DataModels.Outbox>().AddAsync(outboxEvent, cancellationToken);
    }
}
