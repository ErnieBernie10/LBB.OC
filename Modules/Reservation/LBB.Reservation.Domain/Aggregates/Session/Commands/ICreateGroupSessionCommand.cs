namespace LBB.Reservation.Domain.Aggregates.Session.Commands;

public interface ICreateGroupSessionCommand : ICreateIndividualSessionCommand
{
    public int? Capacity { get; set; }
}
