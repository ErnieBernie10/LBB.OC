using FluentResults;
using LBB.Core;
using LBB.Core.Errors;
using LBB.Core.ValueObjects;
using LBB.Reservation.Domain.Aggregates.Session.Commands;

namespace LBB.Reservation.Domain.Aggregates.Session;

public class Reservation : Entity
{
    public ReservationReference Reference { get; private set; }
    public PersonName Name { get; private set; }
    public int AttendeeCount { get; private set; }
    public EmailAddress Email { get; private set; }
    public PhoneNumber Phone { get; private set; }

    public Reservation(
        ReservationReference reference,
        PersonName name,
        int attendeeCount,
        EmailAddress email,
        PhoneNumber phone
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
        var c = ValidateMemberCount(command.AttendeeCount ?? 1, nameof(command.AttendeeCount));
        var name = PersonName.Create(
            command.Firstname,
            command.Lastname,
            nameof(command.Firstname),
            nameof(command.Lastname)
        );
        var emailAddress = EmailAddress.Create(command.Email, nameof(command.Email));
        var phoneNumber = PhoneNumber.Create(command.PhoneNumber, nameof(command.PhoneNumber));

        var result = Result.Merge(name, emailAddress, phoneNumber, c);
        if (result.IsFailed)
        {
            return result;
        }

        return new Reservation(
            ReservationReference.New,
            name.Value,
            c.ValueOrDefault,
            emailAddress.Value,
            phoneNumber.Value
        );
    }

    private static Result<int> ValidateMemberCount(int guestCount, string guestCountPropertyName)
    {
        if (guestCount < 1)
            return Result.Fail(new GreaterThanError(guestCountPropertyName, 1));

        return Result.Ok(guestCount);
    }
}
