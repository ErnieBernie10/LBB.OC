using FluentResults;

namespace LBB.Core.Errors;

public abstract class DomainValidationError : Error
{
    public string PropertyName { get; }
    public abstract string ErrorCode { get; }

    protected DomainValidationError(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
    }
}
