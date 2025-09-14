namespace LBB.Core.Contracts;

public interface IAggregateStore<T, in TIdentifier>
    where T : AggregateRoot
{
    Task<T?> GetByIdAsync(TIdentifier id);
}
