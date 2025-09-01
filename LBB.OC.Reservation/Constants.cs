namespace LBB.OC.Reservation;

public class Constants
{
    public const string ModuleBasePath = "reservation";

    public static class Roles
    {
        public const string ReservationManager = "reservation.manager";
    }

    public static class Policies
    {
        public const string ManageReservations = nameof(ManageReservations);
    }
}