using Dapper;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
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
    private readonly IDbConnectionAccessor _connectionAccessor;

    public ReservationMigrations(IDbConnectionAccessor connectionAccessor)
    {
        _connectionAccessor = connectionAccessor;
    }

    public async Task<int> CreateAsync()
    {
        using var connection = _connectionAccessor.CreateConnection();
        await connection.OpenAsync();

        using var command = connection.CreateCommand();

        // Enable foreign keys in SQLite
        command.CommandText = @"PRAGMA foreign_keys = ON;";
        await command.ExecuteNonQueryAsync();

        // Create Sessions table
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS Sessions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type INTEGER NOT NULL DEFAULT 0,
    Capacity INTEGER NOT NULL DEFAULT 1,
    Title TEXT NOT NULL,
    Description TEXT NOT NULL DEFAULT '',
    Start DATETIME NOT NULL,
    End DATETIME NOT NULL,
    Location TEXT NOT NULL DEFAULT '',
    UserId TEXT NOT NULL
);";
        await command.ExecuteNonQueryAsync();

        // Create Reservations table with foreign key
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS Reservations (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Firstname TEXT NOT NULL DEFAULT '',
    Lastname TEXT NOT NULL DEFAULT '',
    Email TEXT NOT NULL,
    Phone TEXT NOT NULL DEFAULT '',
    AttendeeCount INTEGER NOT NULL DEFAULT 1,
    SessionId INTEGER NOT NULL,
    Reference TEXT NOT NULL UNIQUE,
    FOREIGN KEY (SessionId) REFERENCES Sessions(Id) ON DELETE CASCADE
);";
        await command.ExecuteNonQueryAsync();

        return 1;
    }
}
