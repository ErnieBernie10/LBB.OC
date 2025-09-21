using System.Text.Json;
using LBB.Core.Mediator;
using LBB.OC.Outbox.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.BackgroundTasks;

namespace LBB.OC.Outbox;

public class OutboxMessageRelay : IBackgroundTask
{
    public async Task DoWorkAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken
    )
    {
        var dbContext = serviceProvider.GetRequiredService<OutboxDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<OutboxMessageRelay>>();

        // Fetch unprocessed outbox events
        var messages = await dbContext
            .Outboxes.Where(x => x.ProcessedAt == null)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                // Resolve the event type
                var eventType = NotificationStore.GetTypeByName(message.Type);
                if (eventType == null)
                {
                    logger.LogError($"Failed to resolve event type {message.Type}");
                    continue;
                }

                // Deserialize into the correct type
                var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType);
                if (domainEvent == null)
                    continue;

                var handlerInterfaceType =
                    typeof(IOutOfProcessNotificationHandler<>).MakeGenericType(eventType);
                var handlers = serviceProvider.GetServices(handlerInterfaceType).ToArray();

                foreach (var handler in handlers)
                {
                    // Call HandleAsync(domainEvent, ct) via reflection
                    var method = handlerInterfaceType.GetMethod("HandleAsync")!;
                    await (Task)method.Invoke(handler, [domainEvent, cancellationToken])!;
                }

                // Mark as processed
                message.ProcessedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                // TODO: log and optionally retry later
                Console.WriteLine($"Failed to process outbox message {message.Id}: {ex}");
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
