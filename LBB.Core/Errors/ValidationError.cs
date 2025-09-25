using FluentValidation.Results;

namespace LBB.Core.Errors;

public class ValidationError : FluentResults.Error
{
    public ValidationResult Result { get; }

    public ValidationError(ValidationResult result)
    {
        Result = result;
    }
}
