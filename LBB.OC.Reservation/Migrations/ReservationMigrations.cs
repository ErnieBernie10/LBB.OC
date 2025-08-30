using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.Environment.Shell.Scope;
using YesSql;
using YesSql.Sql;

namespace LBB.OC.Reservation.Migrations;

public class ReservationMigrations : DataMigration
{
    public async Task<int> CreateAsync()
    {
        await SchemaBuilder.CreateTableAsync("Sessions", table => table
            .Column<int>("Id", column => column.PrimaryKey().Identity())
            .Column<short>("Type", column => column.NotNull().WithDefault(0))
            .Column<int>("Capacity", column => column.NotNull().WithDefault(1))
            .Column<string>("Title", column => column.NotNull())
            .Column<string>("Description", column => column.WithDefault(""))
            .Column<DateTime>("Start", column => column.NotNull())
            .Column<DateTime>("End", column => column.NotNull())
            .Column<string>("Location", column => column.WithDefault(""))
            .Column<string>("UserId", column => column.NotNull())
        );

        await SchemaBuilder.CreateTableAsync("Reservations", table => table
            .Column<int>("Id", column => column.PrimaryKey().Identity())
            .Column<string>("Firstname", column => column.WithDefault(""))
            .Column<string>("Lastname", column => column.WithDefault(""))
            .Column<string>("Email", column => column.NotNull())
            .Column<string>("Phone", column => column.WithDefault(""))
            .Column<int>("AttendeeCount", column => column.NotNull().WithDefault(1))
            .Column<string>("Reference", column =>
            {
                column.Unique();
                column.NotNull();
                column.WithLength(20);
            })
        );

        await SchemaBuilder.CreateForeignKeyAsync("FK_Session_User", "Session", ["UserId"], "UserIndex", ["Id"]);
        return 1;
    }
}