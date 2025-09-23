using LBB.Core.Mediator;

namespace LBB.Reservation.Application.Features.SessionFeature.Events;

public record SessionCancelled(int SessionId) : INotification;
