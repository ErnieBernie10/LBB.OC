using FluentResults;

namespace LBB.Core.ValueObjects;

public sealed class Location : ValueObject<Location, string>
{
    public const int MaxLength = 200;

    private Location(string description)
        : base(description) { }

    public string Description => Value;

    public static Result<Location> Create(string description)
    {
        return Validate(description, ValidateDescription, value => new Location(value));
    }

    private static Result ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Fail("Location description cannot be empty");

        if (description.Length > MaxLength)
            return Result.Fail(
                $"Location description cannot be longer than {MaxLength} characters"
            );

        return Result.Ok();
    }
}
