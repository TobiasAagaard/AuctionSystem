using System.Data;
using Npgsql;

namespace AuctionSystem.Modules.Vehicles.Api.Database;

public sealed class NpgsqlVehicleDbConnectionFactory : IVehicleDbConnectionFactory
{

    private readonly string _connectionString;

    public NpgsqlVehicleDbConnectionFactory(string connectionString)
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
