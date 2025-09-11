namespace LBB.Core.Errors;

public class NotEmptyError : DomainValidationError
{
    public NotEmptyError(string propertyName)
        : base(propertyName, $"'{propertyName}' cannot be empty.") { }

    public override string ErrorCode => nameof(NotEmptyError);
}
