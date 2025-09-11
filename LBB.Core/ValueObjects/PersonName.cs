using System.Runtime.CompilerServices;
using FluentResults;
using LBB.Core.Errors;

namespace LBB.Core.ValueObjects;

public sealed class PersonName : ValueObject<PersonName, string>
{
    public const int MaxFirstnameLength = 200;
    public const int MaxLastnameLength = 200;

    public string Firstname { get; }
    public string Lastname { get; }

    public override string Value => $"{Firstname} {Lastname}";

    private PersonName(string firstname, string lastname)
    {
        Firstname = firstname;
        Lastname = lastname;
    }

    public static Result<PersonName> Create(
        string firstname,
        string lastname,
        string firstnamePropertyName,
        string lastnamePropertyName
    )
    {
        var errors = new List<IError>();

        if (string.IsNullOrWhiteSpace(firstname))
            errors.Add(new NotEmptyError(firstnamePropertyName));
        else if (firstname.Length > MaxFirstnameLength)
            errors.Add(new LengthExceededError(firstnamePropertyName, MaxFirstnameLength));

        if (string.IsNullOrWhiteSpace(lastname))
            errors.Add(new NotEmptyError(lastnamePropertyName));
        else if (lastname.Length > MaxLastnameLength)
            errors.Add(new LengthExceededError(lastnamePropertyName, MaxLastnameLength));

        return errors.Count > 0 ? Result.Fail(errors) : Result.Ok();
    }

    public override string ToString() => $"{Firstname} {Lastname}";
}
