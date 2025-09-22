namespace LBB.Reservation.Infrastructure.DataModels;

public partial class Session
{
    public int AttendeeCount => Reservations.Sum(r => r.AttendeeCount);
}
