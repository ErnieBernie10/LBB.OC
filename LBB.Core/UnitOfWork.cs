using LBB.Core;
using LBB.Core.Contracts;
using LBB.Core.Mediator;

namespace LBB.Core;

public class UnitOfWork : IUnitOfWork
{
    private readonly HashSet<AggregateRoot> _aggregateRoots = [];

    private IMediator _mediator;

    public UnitOfWork(IMediator mediator)
    {
        if (mediator == null)
            throw new ArgumentNullException(nameof(mediator));
        _mediator = mediator;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(_aggregateRoots.Select(a => _mediator.DispatchDomainEventsAsync(a)));
        foreach (var aggregateRoot in _aggregateRoots)
            aggregateRoot.ClearDomainEvents();
        _aggregateRoots.Clear();
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        foreach (var aggregateRoot in _aggregateRoots)
            aggregateRoot.ClearDomainEvents();
        _aggregateRoots.Clear();
        return Task.CompletedTask;
    }

    public void RegisterChange(AggregateRoot aggregate)
    {
        _aggregateRoots.Add(aggregate);
    }
}
