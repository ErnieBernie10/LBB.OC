using LBB.Core.Mediator;

namespace LBB.Core;

public abstract class AggregateRoot : Entity
{
    public int Id { get; set; }

    public abstract IEnumerable<INotification> GatherDomainEvents();
}
