using LBB.Core.Mediator;

namespace LBB.Reservation.Domain.Aggregates.Session.Events;

public record SessionCreatedEvent(Session Session) : INotification;
