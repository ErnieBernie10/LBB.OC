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
    private const int BatchSize = 50;
    private const int MaxJobRetries = 3;
    private const int MaxInProcessRetries = 3;

    public async Task DoWorkAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken
    )
    {
        var dbContext = serviceProvider.GetRequiredService<OutboxDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<OutboxMessageRelay>>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var messages = await dbContext
                .Outboxes.Where(x => x.ProcessedAt == null && (x.RetryCount ?? 0) < MaxJobRetries)
                .OrderBy(x => x.CreatedAt)
                .Take(BatchSize)
                .ToListAsync(cancellationToken);

            if (messages.Count == 0)
            {
                break;
            }

            foreach (var message in messages)
            {
                var success = false;

                for (int attempt = 1; attempt <= MaxInProcessRetries && !success; attempt++)
                {
                    try
                    {
                        var eventType = NotificationStore.GetTypeByName(message.Type);
                        if (eventType == null)
                        {
                            logger.LogError("Failed to resolve event type {Type}", message.Type);
                            break; // don’t bother retrying in-process
                        }

                        var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType);
                        if (domainEvent == null)
                        {
                            logger.LogWarning(
                                "Failed to deserialize message {MessageId}",
                                message.Id
                            );
                            break; // permanent failure
                        }

                        var handlerInterfaceType =
                            typeof(IOutboxNotificationHandler<>).MakeGenericType(eventType);
                        var handlers = serviceProvider.GetServices(handlerInterfaceType).ToArray();

                        foreach (var handler in handlers)
                        {
                            var method = handlerInterfaceType.GetMethod("HandleAsync")!;
                            await (Task)
                                method.Invoke(
                                    handler,
                                    new object[] { domainEvent, cancellationToken }
                                )!;
                        }

                        // Success → mark processed
                        message.ProcessedAt = DateTime.UtcNow;
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(
                            ex,
                            "Attempt {Attempt}/{MaxAttempts} failed for message {MessageId}",
                            attempt,
                            MaxInProcessRetries,
                            message.Id
                        );

                        if (attempt < MaxInProcessRetries)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken); // small delay before retry
                        }
                    }
                }

                if (!success)
                {
                    // Let the next job run handle it again
                    message.RetryCount = (message.RetryCount ?? 0) + 1;

                    logger.LogError(
                        "Message {MessageId} failed after {Attempts} in-process retries. "
                            + "Retry count now {RetryCount}/{MaxJobRetries}",
                        message.Id,
                        MaxInProcessRetries,
                        message.RetryCount,
                        MaxJobRetries
                    );
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
