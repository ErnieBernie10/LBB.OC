namespace LBB.Reservation.Domain.Aggregates.Session.Commands;

public interface IUpdateSessionTimeSlotCommand
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
