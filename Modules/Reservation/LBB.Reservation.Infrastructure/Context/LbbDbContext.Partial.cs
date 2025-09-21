using LBB.OC.Outbox;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.Context;

public partial class LbbDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureOutbox();
    }
}
