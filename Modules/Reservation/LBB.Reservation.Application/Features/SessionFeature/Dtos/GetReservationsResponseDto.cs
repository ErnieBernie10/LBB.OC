namespace LBB.Reservation.Application.Features.SessionFeature.Dtos;

public class GetReservationsResponseDto
{
    public string Reference { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public int AttendeeCount { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}
