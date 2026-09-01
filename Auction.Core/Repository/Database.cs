using Microsoft.Extensions.Configuration;
using Npgsql;

public partial class Database
{
    /// <summary>
    /// Application configuration loaded from appsettings.Local.json.
    /// Built once at type initialization and reused across all GetConnection calls.
    /// </summary>
    private static readonly IConfiguration _config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.Local.json", optional: false, reloadOnChange: false)
        .Build();

    /// <summary>
    /// Opens and returns a new NpgsqlConnection using the "AuctionDb" connection
    /// string read from appsettings.Local.json (ConnectionStrings section). Caller
    /// owns the connection and is responsible for disposing it (typically via 'using').
    /// </summary>
    private NpgsqlConnection GetConnection()
    {
        string connectionString = _config.GetConnectionString("AuctionDb")!;

        NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}