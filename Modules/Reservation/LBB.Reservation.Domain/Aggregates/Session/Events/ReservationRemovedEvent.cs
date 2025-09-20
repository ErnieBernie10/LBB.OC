using LBB.Core.Mediator;

namespace LBB.Reservation.Domain.Aggregates.Session.Events;

public record ReservationRemovedEvent(Session Session, Reservation Reservation) : INotification;
