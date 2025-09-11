namespace LBB.Core.Errors;

public class InvalidEmailAddressError : DomainValidationError
{
    public InvalidEmailAddressError(string propertyName)
        : base(propertyName, $"{propertyName} is not a valid email address.") { }

    public override string ErrorCode => nameof(InvalidEmailAddressError);
}
