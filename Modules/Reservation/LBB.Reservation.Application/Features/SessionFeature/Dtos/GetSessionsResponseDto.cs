using System.Text.Json.Serialization;
using LBB.Reservation.Domain;

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
    public DateTime? CancelledOn { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Enums.SessionType Type { get; set; }
}
