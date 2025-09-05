namespace LBB.Core;

public interface IUnitOfWorkHandler
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}