using FluentResults;
using LBB.Core.ValueObjects;

namespace LBB.Reservation.Domain.Aggregates.Session;

public class Capacity : ValueObject<Capacity, Capacity.Component>
{
    public record Component(int Max, int Current);

    public Capacity(Component value)
        : base(value) { }

    public bool IsFull => Value.Current >= Value.Max;

    public static Result<Capacity> Create(int max, int current)
    {
        if (max < current)
            return Result.Fail(new CapacityExceededError());

        return new Capacity(new Component(max, current));
    }

    public static Result<Capacity> Create(int max)
    {
        if (max < 0)
            return Result.Fail("Capacity must be positive");

        return new Capacity(new Component(max, 0));
    }

    public static Result<Capacity> Create()
    {
        return new Capacity(new Component(int.MaxValue, 0));
    }

    public Result<Capacity> Add(int memberCount)
    {
        if (memberCount < 0)
            return Result.Fail("Member count cannot be negative");

        if (memberCount > Value.Max - Value.Current)
            return Result.Fail(new CapacityExceededError());

        var newComponent = Value with { Current = Value.Current + memberCount };
        return Result.Ok(new Capacity(newComponent));
    }

    public Result<Capacity> SetMax(int requestCapacity)
    {
        if (requestCapacity <= 0)
            return Result.Fail("Capacity must be positive");

        if (requestCapacity < Value.Current)
            return Result.Fail("Capacity cannot be less than current capacity");

        var newComponent = Value with { Max = requestCapacity };
        return Result.Ok(new Capacity(newComponent));
    }

    public class CapacityExceededError : Error
    {
        public CapacityExceededError()
            : base("Capacity exceeded") { }
    }
}
