using FluentResults;
using LBB.Core.Errors;

namespace LBB.Core.ValueObjects;

public sealed class PersonName : ValueObject<PersonName, PersonName.NameComponents>
{
    public const int MaxFirstnameLength = 200;
    public const int MaxLastnameLength = 200;

    // Record to hold both name components
    public record NameComponents(string Firstname, string Lastname);

    private PersonName(NameComponents components)
        : base(components) { }

    public string Firstname => Value.Firstname;
    public string Lastname => Value.Lastname;

    public static Result<PersonName> Create(string firstname, string lastname)
    {
        return Validate(
            new NameComponents(firstname, lastname),
            ValidateName,
            components => new PersonName(components)
        );
    }

    private static Result ValidateName(NameComponents components)
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(components.Firstname))
            errors.Add(new DomainValidationError("Firstname cannot be empty"));
        else if (components.Firstname.Length > MaxFirstnameLength)
            errors.Add(new DomainValidationError("Firstname too long"));

        if (string.IsNullOrWhiteSpace(components.Lastname))
            errors.Add(new DomainValidationError("Lastname cannot be empty"));
        else if (components.Lastname.Length > MaxLastnameLength)
            errors.Add(new DomainValidationError("Lastname too long"));

        return errors.Count > 0 ? Result.Fail(errors) : Result.Ok();
    }

    public override string ToString() => $"{Firstname} {Lastname}";
}
