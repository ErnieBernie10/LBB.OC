using LBB.Core.Mediator;
using LBB.Reservation.Domain.Aggregates.Session.Dto;

namespace LBB.Reservation.Domain.Aggregates.Session.Events;

public record ReservationConfirmedEvent(ReservationDto Reservation) : INotification;
