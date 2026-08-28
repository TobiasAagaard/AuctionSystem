
namespace Auction.Core.Models;

public abstract class Vehicle
{
      public Vehicle(string name, double kilometers, int year, double basePrice, bool towBar, LicenseType licenseType, double engineSize, double kmPerLiter, FuelType fuelType, EnergyClass energyClass)
    {
        this.Name = name;
        this.Kilometers = kilometers;
        this.Year = year;
        this.BasePrice = basePrice;
        this.TowBar = towBar;
        this.LicenseType = licenseType;
        this.EngineSize = engineSize;
        this.KmPerLiter = kmPerLiter;
        this.FuelType = fuelType;
        this.EnergyClass = energyClass;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public double Kilometers { get; set; }
    public int Year { get; set; } 
    public double BasePrice { get; set; }
    public bool TowBar { get; set; }
    public double EngineSize { get; set; }
    public double KmPerLiter { get; set; } 
    public LicenseType LicenseType { get; set; }
    public FuelType FuelType { get; set; }
    public EnergyClass EnergyClass { get; set; }
    public override string ToString()
    {
        return $"{Name} - {Year} - {Kilometers} km - {BasePrice} kr";
    }
  

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
