using System.Runtime.CompilerServices;
using FluentResults;
using LBB.Core.Errors;
using LBB.Core.ValueObjects;

namespace LBB.Reservation.Domain.Aggregates.Session;

public class Timeslot : ValueObject<Timeslot, (DateTime Start, DateTime End)>
{
    public DateTime Start { get; }
    public DateTime End { get; }
    public TimeSpan Duration => End - Start;

    internal Timeslot(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public static Result<Timeslot> Create(
        DateTime start,
        DateTime end,
        string startProperty,
        string endProperty
    )
    {
        var errors = new List<IError>();

        if (start == default)
            errors.Add(new NotDefaultError(startProperty));

        if (end == default)
            errors.Add(new NotDefaultError(endProperty));

        if (start >= end)
        {
            errors.Add(new LessThanDateError(startProperty, endProperty));
            errors.Add(new GreaterThanDateError(endProperty, startProperty));
        }

        if (end != default && start != default && (end - start).TotalDays > 365)
        {
            errors.Add(new DurationExceededError(startProperty, TimeSpan.FromDays(365)));
            errors.Add(new DurationExceededError(endProperty, TimeSpan.FromDays(365)));
        }

        if (errors.Count > 0)
            return Result.Fail(errors);

        return Result.Ok(new Timeslot(start, end));
    }

    public bool Contains(DateTime dateTime)
    {
        return dateTime >= Start && dateTime <= End;
    }

    public bool IsAdjacent(Timeslot other, TimeSpan tolerance = default)
    {
        if (tolerance == default)
            tolerance = TimeSpan.Zero;

        return Math.Abs((End - other.Start).TotalSeconds) <= tolerance.TotalSeconds
            || Math.Abs((other.End - Start).TotalSeconds) <= tolerance.TotalSeconds;
    }

    public override (DateTime Start, DateTime End) Value => (Start, End);

    public override string ToString()
    {
        return $"{Start:g} - {End:g}";
    }
}
