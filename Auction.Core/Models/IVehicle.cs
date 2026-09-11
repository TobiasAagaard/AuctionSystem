using Auction_Core.Enums;

namespace Auction_Core.Models;

public interface IVehicle
{
    int Id { get; set; }
    string Name { get; set; }
    double Kilometers { get; set; }
    string RegistrationNumber { get; set; }
    int Year { get; set; }
    double BasePrice { get; set; }
    bool TowBar { get; set; }
    double EngineSize { get; set; }
    double KmPerLiter { get; set; }
    LicenseType LicenseType { get; }
    FuelType FuelType { get; }
    EnergyClass EnergyClass { get; }
    string ToString();
}