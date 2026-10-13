using System.Reflection;
using AuctionSystem.Modules.Vehicles.Api;
using AuctionSystem.Modules.Vehicles.Api.Database;
using DbUp;
using DbUp.Engine;

namespace AuctionSystem.Api.Extensions;

internal static class MigrationExtensions
{
    internal static void ApplyMigrations(this WebApplication app)
    {

        string connectionString = app.Configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Database connection is not configured.");

        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        ApplyMigration(connectionString, Schemas.Vehicles, typeof(VehiclesModule).Assembly);
    }

    private static void ApplyMigration(string connectionString, string schema, Assembly assembly)
    {
        UpgradeEngine upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString, schema)
            .WithScriptsEmbeddedInAssembly(
                assembly,
                name => name.Contains(".Database.Migrations.", StringComparison.Ordinal))
            .JournalToPostgresqlTable(schema, "Schema_versions")
            .WithTransactionPerScript()
            .LogToConsole()
            .Build();

        DatabaseUpgradeResult result = upgrader.PerformUpgrade();
        if (!result.Successful)
        {
            throw new InvalidOperationException(
                $"Migration of schema '{schema}' failed in script '{result.ErrorScript?.Name}'.",
                result.Error);
        }
    }
}