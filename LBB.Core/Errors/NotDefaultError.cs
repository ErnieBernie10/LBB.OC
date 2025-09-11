namespace LBB.Core.Errors;

public class NotDefaultError : DomainValidationError
{
    public NotDefaultError(string propertyName)
        : base(propertyName, $"'{propertyName}' should not be default value.") { }

    public override string ErrorCode => nameof(NotDefaultError);
}
