using System.Diagnostics.CodeAnalysis;

namespace LBB.Reservation.Domain.Aggregates.Session.Dto;

public class SessionDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public int Capacity { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public Enums.SessionType Type { get; set; }
    public ICollection<ReservationDto> Reservations { get; set; } = [];

    public SessionDto() { }

    [SetsRequiredMembers]
    public SessionDto(Session session)
    {
        Id = session.Id;
        Title = session.Title;
        Description = session.Description;
        Location = session.Location;
        Capacity = session.Capacity.Max;
        Start = session.Timeslot.Start;
        End = session.Timeslot.End;
        Type = session.SessionType;
        Reservations = session.Reservations.Select(r => new ReservationDto(session.Id, r)).ToList();
    }

    public Session ToEntity()
    {
        return new Session(
            Id,
            Type,
            new Timeslot(Start, End),
            Title,
            Description ?? "",
            Location ?? "",
            new Capacity(Capacity, Reservations.Count),
            Reservations.Select(r => r.ToEntity()).ToList()
        );
    }
}
