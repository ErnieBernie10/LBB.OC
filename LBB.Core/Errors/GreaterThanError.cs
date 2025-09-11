namespace LBB.Core.Errors;

public class GreaterThanError : DomainValidationError
{
    public int Min { get;  }

    public GreaterThanError(string propertyName, int minValue)
        : base(propertyName, $"{propertyName} must be greater than {minValue}.")
    {
        Min = minValue;
    }

    public override string ErrorCode => nameof(GreaterThanError);
}
