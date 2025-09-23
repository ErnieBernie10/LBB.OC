using OrchardCore.Data;
using OrchardCore.Data.Migration;

namespace LBB.OC.Outbox.Migrations;

public class OutboxMigrations : DataMigration
{
    private readonly IDbConnectionAccessor _connectionAccessor;

    public OutboxMigrations(IDbConnectionAccessor connectionAccessor)
    {
        _connectionAccessor = connectionAccessor;
    }

    public async Task<int> CreateAsync()
    {
        await using var connection = _connectionAccessor.CreateConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText =
            @"
CREATE TABLE IF NOT EXISTS Outbox (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    AggregateType TEXT NOT NULL,
    AggregateId TEXT NOT NULL,
    Type TEXT NOT NULL,
    Module TEXT NOT NULL,
    Payload TEXT NOT NULL, -- JSON stored as TEXT
    CreatedAt DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    ProcessedAt DATETIME DEFAULT NULL,
    RetryCount INTEGER DEFAULT 0
);
";

        await command.ExecuteNonQueryAsync();

        await connection.CloseAsync();
        return 1;
    }
}
