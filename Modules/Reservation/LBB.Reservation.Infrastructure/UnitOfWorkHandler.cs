using LBB.Core;
using LBB.Reservation.Infrastructure.Context;

namespace LBB.Reservation.Infrastructure;

public class UnitOfWorkHandler(LbbDbContext context) : IUnitOfWorkHandler
{
    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}