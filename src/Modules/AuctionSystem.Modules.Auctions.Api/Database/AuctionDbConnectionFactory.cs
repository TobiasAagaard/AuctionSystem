using System.Data;
using Npgsql;

namespace AuctionSystem.Modules.Auctions.Api.Database;

public sealed class NpgsqlAuctionsDbConnectionFactory 
{

    private readonly string _connectionString;

    public NpgsqlAuctionsDbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);
        return connection;
    }

}