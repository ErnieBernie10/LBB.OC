namespace LBB.Reservation.Domain.Aggregates.Session.Commands;

public interface IAddReservationCommand
{
    public string? Firstname { get; set; }

    public string? Lastname { get; set; }
    public int? AttendeeCount { get; set; }

    public string Email { get; set; }

    public string? PhoneNumber { get; set; }
}
