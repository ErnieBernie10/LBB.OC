namespace LBB.Reservation.Infrastructure;

public static class DbConstraints
{
    public static class Session
    {
        public const int MaxTitleLength = 200;
        public const int MaxDescriptionLength = 2000;
        public const int MaxLocationLength = 2000;
    }

    public static class Reservation
    {
        public const int MaxFirstnameLength = 200;
        public const int MaxLastnameLength = 200;
        public const int MaxEmailLength = 200;
        public const int MaxPhoneNumberLength = 20;
    }
}
