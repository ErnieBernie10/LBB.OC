namespace LBB.Core.Errors;

public class LessThanDateError : DomainValidationError
{
    public string Property2Name { get; }

    public LessThanDateError(string propertyName, string property2Name)
        : base(propertyName, $"{propertyName} must be before {property2Name}")
    {
        Property2Name = property2Name;
    }

    public override string ErrorCode => nameof(LessThanDateError);
}
