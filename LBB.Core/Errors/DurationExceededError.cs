namespace LBB.Core.Errors;

public class DurationExceededError : DomainValidationError
{
    public TimeSpan Duration { get; }

    public DurationExceededError(string propertyName, TimeSpan duration)
        : base(propertyName, $"{propertyName} duration exceeded.")
    {
        Duration = duration;
    }

    public override string ErrorCode => nameof(DurationExceededError);
}
