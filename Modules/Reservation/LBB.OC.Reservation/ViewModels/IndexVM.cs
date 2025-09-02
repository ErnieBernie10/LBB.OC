using LBB.OC.Reservation.ViewModels.Session;
using LBB.Reservation.Infrastructure.DataModels;

namespace LBB.OC.Reservation.ViewModels;

public record IndexVM(List<LBB.Reservation.Infrastructure.DataModels.Session> Sessions, CreateSessionVM CreateSessionVM);