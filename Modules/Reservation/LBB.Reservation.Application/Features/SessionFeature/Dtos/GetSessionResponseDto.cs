namespace LBB.Reservation.Application.Features.SessionFeature.Dtos;

public class GetSessionResponseDto : GetSessionsResponseDto
{
    public IEnumerable<GetReservationsResponseDto> Reservations { get; set; }
}
