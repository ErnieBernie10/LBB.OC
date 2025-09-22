using LBB.Core.Contracts;
using LBB.Core.Mediator;

namespace LBB.Core;

public class UnitOfWork : IUnitOfWork
{
    private readonly HashSet<AggregateRoot> _aggregateRoots = [];

    private IMediator _mediator;
    private readonly IUnitOfWorkHandler _handler;

    public UnitOfWork(IMediator mediator, IUnitOfWorkHandler handler)
    {
        if (mediator == null)
            throw new ArgumentNullException(nameof(mediator));
        _mediator = mediator;
        _handler = handler;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(_aggregateRoots.Select(a => _mediator.DispatchDomainEventsAsync(a)));
        foreach (var aggregateRoot in _aggregateRoots)
            aggregateRoot.ClearDomainEvents();
        await _handler.CommitAsync(cancellationToken);
        _aggregateRoots.Clear();
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        foreach (var aggregateRoot in _aggregateRoots)
            aggregateRoot.ClearDomainEvents();
        await _handler.RollbackAsync(cancellationToken);
        _aggregateRoots.Clear();
    }

    public async Task BeginAsync(CancellationToken cancellationToken = default)
    {
        await _handler.BeginAsync(cancellationToken);
    }

    public void RegisterChange(AggregateRoot aggregate)
    {
        _aggregateRoots.Add(aggregate);
    }
}
