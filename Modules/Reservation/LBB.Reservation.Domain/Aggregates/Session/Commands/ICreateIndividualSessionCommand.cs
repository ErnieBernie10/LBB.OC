namespace LBB.Reservation.Domain.Aggregates.Session.Commands;

public interface ICreateIndividualSessionCommand
{
    public Enums.SessionType Type { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Location { get; set; }
}