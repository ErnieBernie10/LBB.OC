using LBB.Core.Errors;

namespace LBB.Reservation.Application.Features.SessionFeature.Errors;

public class ReservationExistsError : BusinessError
{
    public ReservationExistsError()
        : base("Reservation already exists with this email address.") { }

    public override string ErrorCode => nameof(ReservationExistsError);
}
