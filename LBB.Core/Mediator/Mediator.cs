using System.Reflection;
using LBB.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace LBB.Core.Mediator;

public class Mediator(IServiceProvider provider) : IMediator
{
    public async Task<TResult> SendCommandAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default
    )
        where TCommand : ICommand<TResult>
    {
        var handler = provider.GetService<ICommandHandler<TCommand, TResult>>();
        if (handler == null)
        {
            throw new InvalidOperationException(
                $"No handler registered for {typeof(TCommand).Name}"
            );
        }

        var behaviors = provider.GetServices<IPipelineBehavior<TCommand, TResult>>().Reverse();

        Func<Task<TResult>> handlerDelegate = () => handler.HandleAsync(command, cancellationToken);
        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.HandleAsync(command, next, cancellationToken);
        }

        return await handlerDelegate();
    }

    public async Task<TResult> SendQueryAsync<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default
    )
        where TQuery : IQuery<TResult>
    {
        var handler = provider.GetService<IQueryHandler<TQuery, TResult>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for {typeof(TQuery).Name}");
        }

        var behaviors = provider.GetServices<IPipelineBehavior<TQuery, TResult>>().Reverse();
        Func<Task<TResult>> handlerDelegate = () => handler.HandleAsync(query, cancellationToken);
        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.HandleAsync(query, next, cancellationToken);
        }

        return await handlerDelegate();
    }

    // Generic PublishAsync that works when you know TNotification at compile time
    public async Task PublishAsync<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default
    )
        where TNotification : INotification
    {
        if (notification is null)
            throw new ArgumentNullException(nameof(notification));

        var inProcessHandlers = provider
            .GetServices<IInProcessNotificationHandler<TNotification>>()
            .ToArray();

        var outOfProcessHandlers = provider
            .GetServices<IOutOfProcessNotificationHandler<TNotification>>()
            .ToArray();

        if (inProcessHandlers.Length == 0)
        {
            throw new InvalidOperationException(
                $"No handler registered for {typeof(TNotification).Name}"
            );
        }

        if (outOfProcessHandlers.Length > 0)
        {
            var outboxService = provider.GetRequiredService<IOutboxService>();
            await outboxService.PublishAsync(
                "",
                "",
                typeof(TNotification).Name,
                notification,
                cancellationToken
            );
        }

        var tasks = new Task[inProcessHandlers.Length];
        for (var i = 0; i < inProcessHandlers.Length; i++)
        {
            tasks[i] = inProcessHandlers[i].HandleAsync(notification, cancellationToken);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    // Cached non-generic overload that dispatches to the generic method using a compiled delegate per notification type
    public Task PublishAsync(
        INotification notification,
        CancellationToken cancellationToken = default
    )
    {
        if (notification is null)
            throw new ArgumentNullException(nameof(notification));

        var dispatcher = s_publishDispatchers.GetOrAdd(
            notification.GetType(),
            static t => BuildPublishDispatcher(t)
        );

        return dispatcher(this, notification, cancellationToken);
    }

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<
        System.Type,
        Func<Mediator, INotification, CancellationToken, Task>
    > s_publishDispatchers = new();

    private static Func<Mediator, INotification, CancellationToken, Task> BuildPublishDispatcher(
        System.Type notificationType
    )
    {
        var mediatorParam = System.Linq.Expressions.Expression.Parameter(typeof(Mediator), "m");
        var notificationParam = System.Linq.Expressions.Expression.Parameter(
            typeof(INotification),
            "n"
        );
        var ctParam = System.Linq.Expressions.Expression.Parameter(typeof(CancellationToken), "ct");

        var genericPublishMethod = typeof(Mediator)
            .GetMethods(
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
            )
            .Single(m =>
                m.Name == nameof(PublishAsync)
                && m.IsGenericMethodDefinition
                && m.GetGenericArguments().Length == 1
                && m.GetParameters().Length == 2
            );

        var closedPublishMethod = genericPublishMethod.MakeGenericMethod(notificationType);

        var call = System.Linq.Expressions.Expression.Call(
            mediatorParam,
            closedPublishMethod,
            System.Linq.Expressions.Expression.Convert(notificationParam, notificationType),
            ctParam
        );

        var lambda = System.Linq.Expressions.Expression.Lambda<
            Func<Mediator, INotification, CancellationToken, Task>
        >(call, mediatorParam, notificationParam, ctParam);

        return lambda.Compile();
    }

    public Task DispatchDomainEventsAsync(AggregateRoot root)
    {
        if (root is null)
            throw new ArgumentNullException(nameof(root));

        var domainEvents = root.GatherDomainEvents();
        var tasks = domainEvents.Select(e => PublishAsync(e, CancellationToken.None));
        return Task.WhenAll(tasks);
    }
}
