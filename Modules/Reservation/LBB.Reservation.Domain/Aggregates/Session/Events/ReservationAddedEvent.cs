using FluentResults;
using LBB.Core.Contracts;
using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Dto;

namespace LBB.Reservation.Domain.Aggregates.Session.Events;

public record ReservationAddedEvent(SessionDto Session, ReservationDto Reservation) : INotification;
