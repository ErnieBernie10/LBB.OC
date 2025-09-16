using LBB.Core.Errors;

namespace LBB.Reservation.Domain.Aggregates.Session.Errors;

public class SessionCapacityExceededError : DomainValidationError
{
    public SessionCapacityExceededError(string propertyName)
        : base(propertyName, "Session capacity would be exceeded.") { }

    public override string ErrorCode => nameof(SessionCapacityExceededError);
}
