using System.Data;
using LBB.Core;
using LBB.Reservation.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace LBB.Reservation.Infrastructure;

public class UnitOfWorkHandler(LbbDbContext context) : IUnitOfWorkHandler
{
    IDbContextTransaction? transaction;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        if (transaction != null)
            await transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (transaction != null)
            await transaction.RollbackAsync(cancellationToken);
    }

    public async Task BeginAsync(CancellationToken cancellationToken = default)
    {
        transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }
}
