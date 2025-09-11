using FluentResults;

namespace LBB.Core.ValueObjects;

public abstract class ValueObject<T, TValue>
    where T : ValueObject<T, TValue>
{
    public abstract TValue Value { get; }

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
}
