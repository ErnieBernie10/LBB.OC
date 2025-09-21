using Microsoft.EntityFrameworkCore;

namespace LBB.OC.Outbox;

public static class OutboxModelBuilderExtensions
{
    public static void ConfigureOutbox(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataModels.Outbox>(entity =>
        {
            entity.ToTable("Outbox");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AggregateType).IsRequired();
            entity.Property(e => e.AggregateId).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Payload).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
