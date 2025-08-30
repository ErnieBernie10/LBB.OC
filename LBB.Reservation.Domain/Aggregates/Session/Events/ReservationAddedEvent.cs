using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Mediator;

namespace LBB.Reservation.Domain.Aggregates.Session.Events;

public record ReservationAddedEvent(Session Session, Reservation Reservation) : INotification;
