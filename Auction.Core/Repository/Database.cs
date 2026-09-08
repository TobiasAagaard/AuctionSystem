using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Auction_Core.Repository;

public partial class Database
{
    /// <summary>
    /// Application configuration loaded from appsettings.Local.json.
    /// Built once at type initialization and reused across all GetConnection calls.
    /// </summary>
    private static readonly IConfiguration _config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
        .Build();

    private readonly string? _connectionString;

    public Database(string? connectionString = null)
    {
        _connectionString = connectionString;
    }

    public async Task<NpgsqlConnection> GetConnection()
    {
        string connectionString = _connectionString
            ?? _config.GetConnectionString("AuctionDb")
            ?? throw new InvalidOperationException(
                "No database connection string was configured. Add ConnectionStrings:AuctionDb " +
                "to appsettings.Local.json or pass a connection string to Database.");

        NpgsqlConnection connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync();
        return connection;
    }
}