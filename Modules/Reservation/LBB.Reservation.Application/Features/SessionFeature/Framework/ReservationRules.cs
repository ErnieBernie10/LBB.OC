using FluentResults;
using LBB.Core;
using LBB.Core.Contracts;
using LBB.Core.Enums;
using LBB.Reservation.Application.Features.SessionFeature.Errors;
using LBB.Reservation.Domain;
using LBB.Reservation.Infrastructure.DataModels;

namespace LBB.Reservation.Application.Features.SessionFeature.Framework;

public record ReservationRuleContext(IReservationInput Input, Session Session, EditType EditType);

public static class ReservationRules
{
    public class CanReserveRule : IBusinessRule<ReservationRuleContext>
    {
        public string Name => nameof(CanReserveRule);

        public async Task<Result> CheckAsync(
            ReservationRuleContext input,
            CancellationToken ct = default
        )
        {
            var session = input.Session;
            var command = input.Input;
            switch ((Enums.SessionType)session.Type)
            {
                case Enums.SessionType.Individual
                    when session.Reservations.Any(r => r.CancelledOn == null)
                        && input.EditType == EditType.Add:
                    return Result.Fail(new CapacityExceededError(nameof(command.AttendeeCount)));
                case Enums.SessionType.Group
                    when session
                        .Reservations.Where(r => r.CancelledOn == null)
                        .Sum(r => r.AttendeeCount) + (command.AttendeeCount ?? 1)
                        > session.Capacity:
                    return Result.Fail(new CapacityExceededError(nameof(command.AttendeeCount)));
                default:
                    return Result.Ok();
            }
        }
    }

    public class NoExistingReservationWithEmail : IBusinessRule<ReservationRuleContext>
    {
        public string Name => nameof(NoExistingReservationWithEmail);

        public async Task<Result> CheckAsync(
            ReservationRuleContext input,
            CancellationToken ct = default
        )
        {
            var session = input.Session;
            var command = input.Input;
            if (
                session
                    .Reservations.Where(r => r.CancelledOn == null)
                    .Any(r => r.Email == command.Email)
            )
                return new ReservationExistsError();

            return Result.Ok();
        }
    }
}
