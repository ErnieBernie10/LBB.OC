using FluentResults;
using LBB.Core;
using LBB.Core.ValueObjects;

namespace LBB.Reservation.Domain.Aggregates.Session;

public class Reservation : Entity
{
    public ReservationReference Reference { get; private set; }
    public PersonName Name { get; private set; }
    public int AttendeeCount { get; private set; }
    public EmailAddress Email { get; private set; }
    public PhoneNumber Phone { get; private set; }

    internal Reservation(
        string reference,
        string firstname,
        string lastname,
        int attendeeCount,
        string email,
        string phone
    )
    {
        Reference = new ReservationReference(reference);
        Name = PersonName.Create(firstname, lastname).Value;
        AttendeeCount = attendeeCount;
        Email = EmailAddress.Create(email).Value;
        Phone = PhoneNumber.Create(phone).Value;
    }

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

    public static Result<Reservation> Create(
        string firstname,
        string lastname,
        string email,
        string phone,
        int guestCount = 1
    )
    {
        var c = ValidateGuestCount(guestCount);
        var name = PersonName.Create(firstname, lastname);
        var emailAddress = EmailAddress.Create(email);
        var phoneNumber = PhoneNumber.Create(phone);

        var result = Result.Merge(name, emailAddress, phoneNumber, c);
        if (result.IsFailed)
        {
            return result;
        }

        return new Reservation(
            ReservationReference.New,
            name.Value,
            guestCount,
            emailAddress.Value,
            phoneNumber.Value
        );
    }

    private static Result<int> ValidateGuestCount(int guestCount)
    {
        if (guestCount < 1)
            return Result.Fail("Guest count must be at least 1");

        return Result.Ok(guestCount);
    }
}
