namespace LBB.Reservation.Application.Features.SessionFeature.Dtos;

public class GetSessionsResponseDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int AttendeeCount { get; set; }
    public string Location { get; set; }
    public int Capacity { get; set; }
    public int Id { get; set; }
}