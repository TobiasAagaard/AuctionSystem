using System.Reflection;
using Auctionly.Modules.Auctions.Api;
using Auctionly.Modules.Auctions.Api.Database;
using DbUp;
using DbUp.Engine;

namespace Auctionly.Api.Extensions;

internal static class MigrationExtensions
{
    internal static void ApplyMigrations(this WebApplication app)
    {

        string connectionString = app.Configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Database connection is not configured.");

        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        ApplyMigration(connectionString, Schemas.Auctions, typeof(AuctionsModule).Assembly);
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