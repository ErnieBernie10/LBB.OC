using System;
using System.Collections.Generic;
using LBB.Reservation.Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace LBB.Reservation.Infrastructure.Context;

public partial class LbbDbContext : DbContext
{
    public LbbDbContext(DbContextOptions<LbbDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DeploymentPlanIndex> DeploymentPlanIndices { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<NotificationDocument> NotificationDocuments { get; set; }

    public virtual DbSet<NotificationNotificationIndex> NotificationNotificationIndices { get; set; }

    public virtual DbSet<DataModels.Reservation> Reservations { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<UserByClaimIndex> UserByClaimIndices { get; set; }

    public virtual DbSet<UserByLoginInfoIndex> UserByLoginInfoIndices { get; set; }

    public virtual DbSet<UserByRoleNameIndex> UserByRoleNameIndices { get; set; }

    public virtual DbSet<UserByRoleNameIndexDocument> UserByRoleNameIndexDocuments { get; set; }

    public virtual DbSet<UserIndex> UserIndices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeploymentPlanIndex>(entity =>
        {
            entity.Property(e => e.Name).UseCollation("NOCASE");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content).UseCollation("NOCASE");
            entity.Property(e => e.Type).UseCollation("NOCASE");
        });

        modelBuilder.Entity<NotificationDocument>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content).UseCollation("NOCASE");
            entity.Property(e => e.Type).UseCollation("NOCASE");
        });

        modelBuilder.Entity<NotificationNotificationIndex>(entity =>
        {
            entity.Property(e => e.Content).UseCollation("NOCASE");
            entity.Property(e => e.NotificationId).UseCollation("NOCASE");
            entity.Property(e => e.UserId).UseCollation("NOCASE");
        });

        modelBuilder.Entity<DataModels.Reservation>(entity =>
        {
            entity.Property(e => e.Email).UseCollation("NOCASE");
            entity.Property(e => e.Firstname)
                .HasDefaultValue("")
                .UseCollation("NOCASE");
            entity.Property(e => e.Lastname)
                .HasDefaultValue("")
                .UseCollation("NOCASE");
            entity.Property(e => e.Phone)
                .HasDefaultValue("")
                .UseCollation("NOCASE");
            entity.Property(e => e.Reference).UseCollation("NOCASE");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.Property(e => e.Description)
                .HasDefaultValue("")
                .UseCollation("NOCASE");
            entity.Property(e => e.Location)
                .HasDefaultValue("")
                .UseCollation("NOCASE");
            entity.Property(e => e.Title).UseCollation("NOCASE");
            entity.Property(e => e.UserId).UseCollation("NOCASE");
        });

        modelBuilder.Entity<UserByClaimIndex>(entity =>
        {
            entity.Property(e => e.ClaimType).UseCollation("NOCASE");
            entity.Property(e => e.ClaimValue).UseCollation("NOCASE");
        });

        modelBuilder.Entity<UserByLoginInfoIndex>(entity =>
        {
            entity.Property(e => e.LoginProvider).UseCollation("NOCASE");
            entity.Property(e => e.ProviderKey).UseCollation("NOCASE");
        });

        modelBuilder.Entity<UserByRoleNameIndex>(entity =>
        {
            entity.Property(e => e.RoleName).UseCollation("NOCASE");
        });

        modelBuilder.Entity<UserIndex>(entity =>
        {
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.NormalizedEmail).UseCollation("NOCASE");
            entity.Property(e => e.NormalizedUserName).UseCollation("NOCASE");
            entity.Property(e => e.UserId).UseCollation("NOCASE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
