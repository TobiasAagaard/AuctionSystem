
namespace Auction.Core.Models;

public abstract class Vehicle
{
    public Vehicle(string name, double kilometers, int year, double basePrice, bool towBar, LicenseType licenseType, double engineSize, double kmPerLiter, FuelType fuelType, EnergyClass energyClass)
    {
        Name = name;
        Kilometers = kilometers;
        Year = year;
        BasePrice = basePrice;
        TowBar = towBar;
        LicenseType = licenseType;
        EngineSize = engineSize;
        KmPerLiter = kmPerLiter;
        FuelType = fuelType;
        EnergyClass = energyClass;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public double Kilometers { get; set; }
    public int Year { get; set; } 
    public double BasePrice { get; set; }
    public bool TowBar { get; set; }
    public LicenseType LicenseType { get; set; }
    public double EngineSize { get; set; }
    public double KmPerLiter { get; set; } 
    public FuelType FuelType { get; set; }
    public EnergyClass EnergyClass { get; set; }


}


public enum FuelType
{
    Diesel,
    Petrol,
    Electric,
    Hydrogen,
}

public enum EnergyClass
{
    A,
    B,
    C,
    D,
}

public enum LicenseType
{
    A,
    B,
    C,
    D,
    BE,
    CE,
    DE,
}
