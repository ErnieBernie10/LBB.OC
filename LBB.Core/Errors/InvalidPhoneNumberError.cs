namespace LBB.Core.Errors;

public class InvalidPhoneNumberError : DomainValidationError
{
    public InvalidPhoneNumberError(string propertyName)
        : base(propertyName, $"{propertyName} is not a valid phone number.") { }

    public override string ErrorCode => nameof(InvalidPhoneNumberError);
}
