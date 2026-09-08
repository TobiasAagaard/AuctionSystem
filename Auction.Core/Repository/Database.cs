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


    public async Task<NpgsqlConnection> GetConnection()
    {
        string connectionString = _config.GetConnectionString("AuctionDb") ?? "Host=localhost;Username=username;Password=password;Database=default_database";


        NpgsqlConnection connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync();
        return connection;
    }
}