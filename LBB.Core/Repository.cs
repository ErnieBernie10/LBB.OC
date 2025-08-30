using LBB.Core.Contracts;

namespace LBB.Core;

public abstract class Repository<T> : IRepository
    where T : AggregateRoot
{
    protected readonly IUnitOfWork UnitOfWork;

    public Repository(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }
}
