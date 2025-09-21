using System.Runtime.Serialization;
using FluentResults;
using LBB.Core;
using LBB.Core.Errors;
using LBB.Core.ValueObjects;
using LBB.Reservation.Domain.Aggregates.Session.Commands;
using LBB.Reservation.Domain.Aggregates.Session.Dto;
using LBB.Reservation.Domain.Aggregates.Session.Events;

namespace LBB.Reservation.Domain.Aggregates.Session;

public class Reservation : Entity
{
    public int Id { get; private set; }
    public ReservationReference Reference { get; private set; }
    public PersonName? Name { get; private set; }
    public int AttendeeCount { get; private set; }
    public EmailAddress Email { get; private set; }
    public PhoneNumber? Phone { get; private set; }
    public bool ConfirmationSent { get; private set; }

    public Reservation(
        int id,
        ReservationReference reference,
        PersonName? name,
        int attendeeCount,
        EmailAddress email,
        PhoneNumber? phone
    )
    {
        Id = id;
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
            0,
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

    public void Confirm()
    {
        ConfirmationSent = true;
        AddDomainEvent(new ReservationConfirmationSentEvent(new ReservationDto(this)));
    }
}
