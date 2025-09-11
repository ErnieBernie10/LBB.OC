using System.Runtime.CompilerServices;
using FluentResults;
using LBB.Core.Errors;

namespace LBB.Core.ValueObjects;

public sealed class Location : ValueObject<Location, string>
{
    private Location(string description)
    {
        Value = description;
    }

    public const int MaxLength = 200;

    public string Description => Value;

    public static Result<Location> Create(string description, string descriptionPropertyName)
    {
        if (description.Length > MaxLength)
            return Result.Fail(new LengthExceededError(descriptionPropertyName, MaxLength));

        return Result.Ok(new Location(description));
    }

    public override string Value { get; }
}
