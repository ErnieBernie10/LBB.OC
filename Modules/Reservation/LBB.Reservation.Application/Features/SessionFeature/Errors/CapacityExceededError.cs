using LBB.Core.Errors;

namespace LBB.Reservation.Application.Features.SessionFeature.Errors;

public class CapacityExceededError : PropertyError
{
    public CapacityExceededError(string propertyName)
        : base("Capacity exceeded", propertyName) { }

    public override string ErrorCode => nameof(CapacityExceededError);
}
