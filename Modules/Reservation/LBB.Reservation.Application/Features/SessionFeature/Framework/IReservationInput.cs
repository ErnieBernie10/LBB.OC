using System.Text.Json.Serialization;
using FluentValidation;
using LBB.Reservation.Domain;
using LBB.Reservation.Infrastructure;

namespace LBB.Reservation.Application.Features.SessionFeature.Framework;

public interface IReservationInput
{
    public string? Firstname { get; set; }

    public string? Lastname { get; set; }
    public int? AttendeeCount { get; set; }

    public string Email { get; set; }

    public string? PhoneNumber { get; set; }
    public int SessionId { get; set; }
}

public class ReservationInputValidator : AbstractValidator<IReservationInput>
{
    public ReservationInputValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty()
            .MaximumLength(DbConstraints.Reservation.MaxEmailLength);
        RuleFor(x => x.PhoneNumber).MaximumLength(DbConstraints.Reservation.MaxPhoneNumberLength);
        RuleFor(x => x.Firstname).MaximumLength(DbConstraints.Reservation.MaxFirstnameLength);
        RuleFor(x => x.Lastname).MaximumLength(DbConstraints.Reservation.MaxLastnameLength);
        RuleFor(x => x.AttendeeCount).GreaterThanOrEqualTo(1);
    }
}
