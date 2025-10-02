using LBB.Core.Mediator;

namespace LBB.Reservation.Application.Features.SessionFeature.Events;

public record ReservationUpdatedEvent(int ReservationId) : INotification;
