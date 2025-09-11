namespace LBB.Core.Errors;

public class GreaterThanDateError : DomainValidationError
{
    public string Property2Name { get; set; }

    public GreaterThanDateError(string propertyName, string property2Name)
        : base(propertyName, $"{propertyName} must be after {property2Name}")
    {
        Property2Name = property2Name;
    }

    public override string ErrorCode => nameof(GreaterThanDateError);
}
