using Auction_Core.Models;

namespace Auction_Core.Repository;

public interface IVehicleRepository
{
   Task<IEnumerable<Vehicle>> GetAllVehiclesByIdAsync(int id);
   Task<Vehicle?> GetVehicleByIdAsync(int id);
   Task AddVehicleAsync(Vehicle vehicle);
   Task UpdateVehicleAsync(Vehicle vehicle);
   Task DeleteVehicleAsync(int id);
}