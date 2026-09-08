using Auction_Core.Models;

namespace Auction_Core.Repository;

public interface IVehicleRepository
{
   Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
   Task<Vehicle?> GetVehicleByIdAsync(int id);
   Task AddVehicleAsync(Vehicle vehicle);
   Task UpdateVehicleAsync(Vehicle vehicle);
   Task DeleteVehicleAsync(int id);
}