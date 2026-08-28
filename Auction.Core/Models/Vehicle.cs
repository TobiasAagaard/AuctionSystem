
namespace Auction.Core.Models;

public class Vehicle
{
    public Vehicle(int id, string name, double kilometers, string registrationNumber, int year, bool towBar, string vehicleType, string engineType, double kmPerLiter, FuelType fuelType)
    {
        Id = id;
        Name = name;
        Kilometers = kilometers;
        RegistrationNumber = registrationNumber;
        Year = year;
        TowBar = towBar;
        VehicleType = vehicleType;
        EngineType = engineType;
        KmPerLiter = kmPerLiter;
        FuelType = fuelType;
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public double Kilometers { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public int Year { get; set; }
    public bool TowBar { get; set; } = false;
    public string VehicleType { get; set; } = null!;
    public string EngineType { get; set; } = null!;
    public double KmPerLiter { get; set; }
    public FuelType FuelType { get; set; }
    public string EnergyClass { get; set; } = null!;

}


public enum FuelType
{
    Diesel,
    Petrol,
    Electric,
    Hydrogen,
}