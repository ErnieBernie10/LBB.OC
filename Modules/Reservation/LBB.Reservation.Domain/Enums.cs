using System.Text.Json.Serialization;

namespace LBB.Reservation.Domain;

public static class Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SessionType
    {
        Individual,
        Group,
    }
}
