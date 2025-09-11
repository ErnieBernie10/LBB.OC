namespace LBB.Core.Errors;

public class GreaterThanError : DomainValidationError
{
    public GreaterThanError(string propertyName, int minValue)
        : base(propertyName, $"{propertyName} must be greater than {minValue}.") { }

    public override string ErrorCode => nameof(GreaterThanError);
}
