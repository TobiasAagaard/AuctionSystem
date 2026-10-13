using System.Data;

namespace AuctionSystem.Modules.Vehicles.Api.Database;

public interface IVehicleDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken token = default);
}
