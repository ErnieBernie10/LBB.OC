using LBB.Core.Mediator;

namespace LBB.Reservation.Domain.Aggregates.Session.Events;

public record ReservationPersistedEvent(Reservation Reservation) : INotification;
