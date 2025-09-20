using LBB.Core.Enums;
using LBB.Core.Mediator;

namespace LBB.Reservation.Domain.Aggregates.Session.Events;

public record SessionPersistedEvent(Session Session, PersistenceState State) : INotification;
