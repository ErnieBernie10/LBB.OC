namespace LBB.Reservation.Domain.Aggregates.Session.Commands;

public interface IUpdateInfoCommand
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public int Capacity { get; set; }
}
