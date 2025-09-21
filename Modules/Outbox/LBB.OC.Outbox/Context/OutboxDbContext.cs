using System;
using System.Collections.Generic;
using LBB.OC.Outbox.DataModels;
using Microsoft.EntityFrameworkCore;

namespace LBB.OC.Outbox.Context;

public partial class OutboxDbContext : DbContext
{
    public OutboxDbContext(DbContextOptions<OutboxDbContext> options)
        : base(options) { }

    public virtual DbSet<DataModels.Outbox> Outboxes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataModels.Outbox>(entity =>
        {
            entity.ToTable("Outbox");

            entity
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("DATETIME");
            entity
                .Property(e => e.ProcessedAt)
                .HasDefaultValueSql("NULL")
                .HasColumnType("DATETIME");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
