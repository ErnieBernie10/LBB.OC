using System.Diagnostics.CodeAnalysis;
using LBB.Core.ValueObjects;

namespace LBB.Reservation.Domain.Aggregates.Session.Dto;

public class ReservationDto
{
    public ReservationDto() { }

    [SetsRequiredMembers]
    public ReservationDto(Reservation reservation)
    {
        Reference = reservation.Reference;
        Firstname = reservation.Name?.Firstname;
        Lastname = reservation.Name?.Lastname;
        AttendeeCount = reservation.AttendeeCount;
        Phone = reservation.Phone?.Value;
        Email = reservation.Email.Value;
        Id = reservation.Id;
        ConfirmationSent = reservation.ConfirmationSent;
    }

    public required string Reference { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public int AttendeeCount { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required int Id { get; set; }
    public bool ConfirmationSent { get; set; }

    public Reservation ToEntity()
    {
        return new Reservation(
            Id,
            new ReservationReference(Reference),
            Firstname == null || Lastname == null ? null : new PersonName(Firstname, Lastname),
            AttendeeCount,
            new EmailAddress(Email),
            Phone == null ? null : new PhoneNumber(Phone),
            ConfirmationSent
        );
    }
}
