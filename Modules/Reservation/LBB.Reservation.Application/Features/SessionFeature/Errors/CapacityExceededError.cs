using LBB.Core.Errors;

namespace LBB.Reservation.Application.Features.SessionFeature.Errors;

public class CapacityExceededError : DomainValidationError
{
    public CapacityExceededError(string propertyName)
        : base(propertyName, "Capacity exceeded") { }

    public override string ErrorCode => nameof(CapacityExceededError);
}
