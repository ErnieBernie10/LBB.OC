namespace LBB.Core.Errors;

public abstract class Error : FluentResults.Error
{
    public Error(string message)
        : base(message) { }

    public abstract string ErrorCode { get; }
}
