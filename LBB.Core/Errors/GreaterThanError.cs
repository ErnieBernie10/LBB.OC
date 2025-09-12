namespace LBB.Core.Errors;

public class GreaterThanError : DomainValidationError
{
    public int GreaterThanValue { get; }

    public GreaterThanError(string propertyName, int greaterThanValue)
        : base(propertyName, $"{propertyName} must be greater than {greaterThanValue}.")
    {
        GreaterThanValue = greaterThanValue;
    }

    public override string ErrorCode => nameof(GreaterThanError);
}
