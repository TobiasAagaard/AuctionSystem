using System.Data;
using Npgsql;

namespace Auctionly.Modules.Auctions.Api.Database;

public sealed class NpgsqlAuctionsDbConnectionFactory(string connectionString)
{

    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(token);
        return connection;
    }

}
