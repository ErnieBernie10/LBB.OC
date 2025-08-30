using FluentResults;

namespace LBB.Core.Errors;

public class ValidationError : Error
{
    public ValidationError(string message)
        : base(message) { }
}
