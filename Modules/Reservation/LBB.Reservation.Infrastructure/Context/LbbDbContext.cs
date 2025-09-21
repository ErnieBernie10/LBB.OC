using System;
using System.Collections.Generic;
using LBB.Reservation.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.Context;

public partial class LbbDbContext : DbContext
{
    public LbbDbContext(DbContextOptions<LbbDbContext> options)
        : base(options) { }

    public virtual DbSet<DataModels.Reservation> Reservations { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DataModels.Reservation>(entity =>
        {
            entity.HasIndex(e => e.Reference, "IX_Reservations_Reference").IsUnique();

            entity.Property(e => e.AttendeeCount).HasDefaultValue(1);
            entity.Property(e => e.CancelledOn).HasColumnType("DATETIME");
            entity.Property(e => e.ConfirmationSentOn).HasColumnType("DATETIME");
            entity
                .Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("DATETIME");
            entity.Property(e => e.Firstname).HasDefaultValue("");
            entity.Property(e => e.Lastname).HasDefaultValue("");
            entity.Property(e => e.Phone).HasDefaultValue("");
            entity.Property(e => e.UpdatedOn).HasColumnType("DATETIME");

            entity
                .HasOne(d => d.Session)
                .WithMany(p => p.Reservations)
                .HasForeignKey(d => d.SessionId);
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.Property(e => e.CancelledOn).HasColumnType("DATETIME");
            entity.Property(e => e.Capacity).HasDefaultValue(1);
            entity
                .Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("DATETIME");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.End).HasColumnType("DATETIME");
            entity.Property(e => e.Location).HasDefaultValue("");
            entity.Property(e => e.PublishedOn).HasColumnType("DATETIME");
            entity.Property(e => e.Start).HasColumnType("DATETIME");
            entity.Property(e => e.UpdatedOn).HasColumnType("DATETIME");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
