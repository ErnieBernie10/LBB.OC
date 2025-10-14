using LBB.OC.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LBB.Reservation.Infrastructure.Context;

public partial class LbbDbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        ConfigureOutbox(modelBuilder);
        ConfigureDateTime(modelBuilder);
    }

    private void ConfigureOutbox(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureOutbox();
    }

    private void ConfigureDateTime(ModelBuilder modelBuilder)
    {
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v, // when saving to DB
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        ); // when reading from DB

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                    property.SetValueConverter(dateTimeConverter);

                if (property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(nullableDateTimeConverter);
            }
        }
    }
}
