namespace LBB.Core.Errors;

public class LengthExceededError : DomainValidationError
{
    public int MaxLength { get; set; }

    public LengthExceededError(string propertyName, int maxLength)
        : base(propertyName, $"'{propertyName}' must be less than {maxLength} characters long.")
    {
        MaxLength = maxLength;
    }

    public override string ErrorCode => nameof(LengthExceededError);
}
