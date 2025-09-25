namespace LBB.Core.Errors;

public abstract class BusinessError : Error
{
    public BusinessError(string message)
        : base(message) { }
}
