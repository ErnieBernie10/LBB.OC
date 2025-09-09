using FluentResults;
using FluentValidation.Results;

namespace LBB.Core.Errors;

public class ValidationError : Error
{
    public ValidationResult Result { get; }

    public ValidationError(ValidationResult result)
    {
        Result = result;
    }
}