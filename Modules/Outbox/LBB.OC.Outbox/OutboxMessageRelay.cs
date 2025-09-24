using System.Diagnostics;
using System.Text.Json;
using LBB.Core.Mediator;
using LBB.OC.Outbox.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.BackgroundTasks;

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
                break;

            foreach (var message in messages)
            {
                var sw = Stopwatch.StartNew();
                var success = false;

                for (int attempt = 1; attempt <= MaxInProcessRetries && !success; attempt++)
                {
                    try
                    {
                        var eventType = NotificationStore.GetTypeByName(message.Type);
                        if (eventType == null)
                        {
                            logger.LogError(
                                "Message {MessageId} failed: unknown type {Type}",
                                message.Id,
                                message.Type
                            );
                            break;
                        }

                        var domainEvent = JsonSerializer.Deserialize(message.Payload, eventType);
                        if (domainEvent == null)
                        {
                            logger.LogWarning(
                                "Message {MessageId} failed: could not deserialize payload",
                                message.Id
                            );
                            break;
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

                        message.ProcessedAt = DateTime.UtcNow;
                        success = true;
                        sw.Stop();

                        logger.LogInformation(
                            "Message {MessageId} processed in {Duration}ms on attempt {Attempt}/{MaxAttempts}",
                            message.Id,
                            sw.ElapsedMilliseconds,
                            attempt,
                            MaxInProcessRetries
                        );
                    }
                    catch (Exception ex)
                    {
                        sw.Stop();
                        logger.LogWarning(
                            ex,
                            "Message {MessageId} attempt {Attempt}/{MaxAttempts} failed after {Duration}ms",
                            message.Id,
                            attempt,
                            MaxInProcessRetries,
                            sw.ElapsedMilliseconds
                        );

                        if (attempt < MaxInProcessRetries)
                            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    }
                }

                if (!success)
                {
                    message.RetryCount = (message.RetryCount ?? 0) + 1;
                    logger.LogError(
                        "Message {MessageId} permanently failed after {InProcessRetries} attempts. RetryCount now {RetryCount}/{MaxJobRetries}",
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
