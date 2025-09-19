namespace LBB.Reservation.Infrastructure;

public static class DbConstraints
{
    public static class Session
    {
        public const int MaxTitleLength = 200;
        public const int MaxDescriptionLength = 2000;
        public const int MaxLocationLength = 2000;
    }
}
