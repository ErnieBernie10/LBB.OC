using FluentResults;
using LBB.Core.Errors;
using LBB.Core.ValueObjects;

namespace LBB.Reservation.Domain.Aggregates.Session;

public class Timeslot : ValueObject<Timeslot, Timeslot.Component>
{
    public record Component(DateTime Start, DateTime End);

    private Timeslot(Component value)
        : base(value) { }

    public TimeSpan Duration => Value.End - Value.Start;

    public static Result<Timeslot> Create(DateTime start, DateTime end)
    {
        if (start == default)
            return Result.Fail(new DomainValidationError("Start time cannot be default value"));

        if (end == default)
            return Result.Fail(new DomainValidationError("End time cannot be default value"));

        if (start > end)
            return Result.Fail(
                new DomainValidationError($"Start time ({start}) must be before end time ({end})")
            );

        if ((end - start).TotalDays > 365)
            return Result.Fail(new DomainValidationError("Timeslot duration cannot exceed one year"));

        return new Timeslot(new Component(start, end));
    }

    public bool Contains(DateTime dateTime)
    {
        return dateTime >= Value.Start && dateTime <= Value.End;
    }

    public bool Overlaps(Timeslot other)
    {
        return Value.Start < other.Value.End && other.Value.Start < Value.End;
    }

    public Result<Timeslot> Intersect(Timeslot other)
    {
        if (!Overlaps(other))
            return Result.Fail(new DomainValidationError("Timeslots do not overlap"));

        var start = Value.Start > other.Value.Start ? Value.Start : other.Value.Start;
        var end = Value.End < other.Value.End ? Value.End : other.Value.End;

        return Create(start, end);
    }

    public bool IsAdjacent(Timeslot other, TimeSpan tolerance = default)
    {
        if (tolerance == default)
            tolerance = TimeSpan.Zero;

        return Math.Abs((Value.End - other.Value.Start).TotalSeconds) <= tolerance.TotalSeconds
            || Math.Abs((other.Value.End - Value.Start).TotalSeconds) <= tolerance.TotalSeconds;
    }

    public override string ToString()
    {
        return $"{Value.Start:g} - {Value.End:g}";
    }
}
