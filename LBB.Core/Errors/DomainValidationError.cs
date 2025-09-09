using FluentResults;

namespace LBB.Core.Errors;

public class DomainValidationError : Error
{
    public DomainValidationError(string message)
        : base(message) { }
}
