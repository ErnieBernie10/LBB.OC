using FluentResults;

namespace LBB.Core.ValueObjects;

public abstract class ValueObject<T, TValue>
    where T : ValueObject<T, TValue>
{
    protected ValueObject(TValue value)
    {
        Value = NormalizeValue(value);
    }

    private static TValue NormalizeValue(TValue value)
    {
        if (value is null)
            return value;

        return value is string strVal ? (TValue)(object)strVal.Trim() : value;
    }

    public TValue Value { get; }

    public static implicit operator TValue(ValueObject<T, TValue> vo) => vo.Value;

    public override bool Equals(object? obj)
    {
        if (obj is not T valueObject)
            return false;

        if (Value is null)
            return valueObject.Value is null;

        return Value.Equals(valueObject.Value);
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? 0;
    }

    public static bool operator ==(ValueObject<T, TValue> left, ValueObject<T, TValue> right)
    {
        if (left is null)
            return right is null;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject<T, TValue> left, ValueObject<T, TValue> right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    protected static Result<T> Validate(
        TValue value,
        Func<TValue, Result> validator,
        Func<TValue, T> factory
    )
    {
        var validationResult = validator(value);
        if (validationResult.IsFailed)
            return Result.Fail(validationResult.Errors);

        return Result.Ok(factory(value));
    }
}
