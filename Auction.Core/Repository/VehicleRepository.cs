using Auction_Core.Models;

namespace Auction_Core.Repository;

public class VehicleRepository : IVehicleRepository
{

    readonly Database _database;

    public VehicleRepository(Database database)
    {
        _database = database;
    }

    internal const string SelectVehicleSql = """
        SELECT  v.id, v.name, v.release_year, v.base_price, v.tow_bar, v.engine_type,
                v.kilometers, v.km_per_liter, v.licence_type, v.fuel_type, hv.weight, hv.height,
                hv.length, st.cargo_capacity, b.seat_count, b.bed_count, b.toilet
    """;
    public async Task AddVehicleAsync(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteVehicleAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Vehicle>> GetAllVehiclesByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Vehicle> GetVehicleByIdAsync(int id)
    {
        throw new NotImplementedException();

    }

    public async Task UpdateVehicleAsync(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }
}