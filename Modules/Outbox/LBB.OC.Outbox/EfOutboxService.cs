using LBB.Core;
using LBB.Core.Contracts;
using LBB.Core.Mediator;
using Microsoft.AspNetCore.SignalR;

namespace LBB.OC.Outbox;

using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public sealed class EfOutboxService<TDbContext> : IOutboxService
    where TDbContext : DbContext
{
    private readonly TDbContext _db;
    private readonly IHubContext<RealtimeHub, IEventClient> _hub;

    public EfOutboxService(TDbContext db, IHubContext<RealtimeHub, IEventClient> hub)
    {
        _db = db;
        _hub = hub;
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
        await _hub
            .Clients.Group(payload.GetType().Name)
            .EventReceived(payload.GetType().Name, payload);
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
