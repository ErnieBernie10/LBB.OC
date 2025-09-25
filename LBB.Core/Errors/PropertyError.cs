namespace LBB.Core.Errors;

public abstract class PropertyError : Error
{
    public string PropertyName { get; }

    public PropertyError(string message, string propertyName)
        : base(message)
    {
        PropertyName = propertyName;
    }
}
