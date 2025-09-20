using LBB.Core.Enums;
using LBB.Core.Mediator;

namespace LBB.Reservation.Domain.Aggregates.Session.Events;

public record ReservationPersistedEvent(Reservation Reservation, PersistenceState State)
    : INotification;
