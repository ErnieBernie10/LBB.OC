using FluentResults;
using LBB.Core;
using LBB.Core.Errors;
using LBB.Core.ValueObjects;
using LBB.Reservation.Domain.Aggregates.Session.Commands;

namespace LBB.Reservation.Domain.Aggregates.Session;

public class Reservation : Entity
{
    public ReservationReference Reference { get; private set; }
    public PersonName? Name { get; private set; }
    public int AttendeeCount { get; private set; }
    public EmailAddress Email { get; private set; }
    public PhoneNumber? Phone { get; private set; }

    internal Reservation(
        string reference,
        string? firstname,
        string? lastname,
        int attendeeCount,
        string email,
        string? phoneNumber
    )
    {
        Reference = ReservationReference.Parse(reference, nameof(reference)).Value;
        Name =
            string.IsNullOrEmpty(firstname) || string.IsNullOrEmpty(lastname)
                ? null
                : PersonName.Create(firstname, lastname, nameof(firstname), nameof(lastname)).Value;
        AttendeeCount = attendeeCount;
        Email = EmailAddress.Create(email, nameof(email)).Value;
        Phone = string.IsNullOrEmpty(phoneNumber)
            ? null
            : PhoneNumber.Create(phoneNumber, nameof(phoneNumber)).Value;
    }

    private Reservation(
        ReservationReference reference,
        PersonName? name,
        int attendeeCount,
        EmailAddress email,
        PhoneNumber? phone
    )
    {
        Reference = reference;
        Name = name;
        AttendeeCount = attendeeCount;
        Email = email;
        Phone = phone;
    }

    public static Result<Reservation> Create(IAddReservationCommand command)
    {
        var attendeeCount = command.AttendeeCount ?? 1;
        var attendeeCountResult = ValidateMemberCount(attendeeCount, nameof(command.AttendeeCount));

        Result<PersonName>? nameResult = null;
        var hasNameProvided =
            !string.IsNullOrWhiteSpace(command.Firstname)
            || !string.IsNullOrWhiteSpace(command.Lastname);
        if (hasNameProvided)
        {
            nameResult = PersonName.Create(
                command.Firstname ?? string.Empty,
                command.Lastname ?? string.Empty,
                nameof(command.Firstname),
                nameof(command.Lastname)
            );
        }

        Result<PhoneNumber>? phoneResult = null;
        if (!string.IsNullOrWhiteSpace(command.PhoneNumber))
        {
            phoneResult = PhoneNumber.Create(command.PhoneNumber!, nameof(command.PhoneNumber));
        }

        var emailResult = EmailAddress.Create(command.Email, nameof(command.Email));

        var mergeResult = Result.Merge(
            nameResult ?? Result.Ok(),
            emailResult,
            phoneResult ?? Result.Ok(),
            attendeeCountResult
        );
        if (mergeResult.IsFailed)
        {
            return mergeResult;
        }

        return new Reservation(
            ReservationReference.New,
            nameResult?.ValueOrDefault,
            attendeeCountResult.ValueOrDefault,
            emailResult.Value,
            phoneResult?.Value
        );
    }

    private static Result<int> ValidateMemberCount(int guestCount, string guestCountPropertyName)
    {
        if (guestCount < 1)
            return Result.Fail(new GreaterThanError(guestCountPropertyName, 1));

        return Result.Ok(guestCount);
    }
}
