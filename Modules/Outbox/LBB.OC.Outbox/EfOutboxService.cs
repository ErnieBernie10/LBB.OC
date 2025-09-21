using LBB.Core.Contracts;

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

    public async Task PublishAsync(
        string aggregateType,
        string aggregateId,
        string type,
        object payload,
        CancellationToken cancellationToken = default
    )
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
}
