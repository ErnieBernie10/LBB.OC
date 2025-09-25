using LBB.Core.Errors;

namespace LBB.Reservation.Application.Features.SessionFeature.Errors;

public class SessionPassedError : BusinessError
{
    public SessionPassedError()
        : base("Session has already started or ended.") { }

    public override string ErrorCode => nameof(SessionPassedError);
}
