using FluentResults;
using LBB.Core.Errors;
using LBB.Core.ValueObjects;

namespace LBB.Reservation.Domain.Aggregates.Session;

public class Capacity : ValueObject<Capacity, (int Max, int Current)>
{
    public int Max { get; }
    public int Current { get; }
    public override (int Max, int Current) Value => (Max, Current);

    public bool IsFull => Current >= Max;

    private Capacity(int max, int current)
    {
        Max = max;
        Current = current;
    }

    public static Result<Capacity> Create(
        int max,
        int current,
        string maxPropertyName,
        string currentPropertyName
    )
    {
        if (max < 0)
            return Result.Fail(new GreaterThanError(maxPropertyName, 0));

        if (current < 0)
            return Result.Fail(new GreaterThanError(currentPropertyName, 0));

        if (max < current)
            return Result.Fail(new CapacityExceededError(currentPropertyName));

        return new Capacity(max, current);
    }

    public static Result<Capacity> Create(int max, string maxPropertyName)
    {
        if (max < 0)
            return Result.Fail(new GreaterThanError(maxPropertyName, 0));

        return new Capacity(max, 0);
    }

    public static Result<Capacity> Create()
    {
        return new Capacity(int.MaxValue, 0);
    }

    public Result<Capacity> Add(int memberCount, string memberCountPropertyName)
    {
        if (memberCount < 0)
            return Result.Fail("Member count cannot be negative");

        if (memberCount > Max - Current)
            return Result.Fail(new CapacityExceededError(memberCountPropertyName));

        var newCurrent = Current + memberCount;
        return Result.Ok(new Capacity(Max, newCurrent));
    }

    public Result<Capacity> SetMax(int requestCapacity)
    {
        if (requestCapacity <= 0)
            return Result.Fail("Capacity must be positive");

        if (requestCapacity < Current)
            return Result.Fail("Capacity cannot be less than current capacity");

        return Result.Ok(new Capacity(requestCapacity, Current));
    }

    public class CapacityExceededError : DomainValidationError
    {
        public CapacityExceededError(string propertyName)
            : base(propertyName, "Capacity exceeded") { }

        public override string ErrorCode => nameof(CapacityExceededError);
    }
}
