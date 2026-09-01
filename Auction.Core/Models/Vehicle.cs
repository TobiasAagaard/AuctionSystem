using Auction_Core.Enums;

namespace Auction_Core.Models;

public abstract class Vehicle
{
      protected Vehicle(string name, double kilometers, string registrationNumber, int year, double basePrice, bool towBar, LicenseType licenseType, double engineSize, double kmPerLiter, FuelType fuelType)
    {
        this.Name = name;
        this.Kilometers = kilometers;
        this.RegistrationNumber = registrationNumber;
        this.Year = year;
        this.BasePrice = basePrice;
        this.TowBar = towBar;
        this.LicenseType = licenseType;
        this.EngineSize = engineSize;
        this.KmPerLiter = kmPerLiter;
        this.FuelType = fuelType;
    }



    protected Vehicle(string name, double kilometers, int year, string registrationNumber, bool towBar, LicenseType licenseType, double engineSize, double kmPerLiter, FuelType fuelType)
    {
        Name = name;
        Kilometers = kilometers;
        Year = year;
        RegistrationNumber = registrationNumber;
        TowBar = towBar;
        LicenseType = licenseType;
        EngineSize = engineSize;
        KmPerLiter = kmPerLiter;
        FuelType = fuelType;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public double Kilometers { get; set; }
    public string RegistrationNumber { get; set; }
    public int Year { get; set; } 
    public double BasePrice { get; set; }
    public bool TowBar { get; set; }
    public double EngineSize { get; set; }
    public double KmPerLiter { get; set; } 
    public virtual LicenseType LicenseType { get; }
    public FuelType FuelType { get; }
    public EnergyClass EnergyClass => GetEnergyClass();
    public abstract override string ToString();
    
    protected EnergyClass GetEnergyClass()
    {
        if (FuelType is FuelType.Electric or FuelType.Hydrogen)
        {
            return EnergyClass.A;
        }

        (double a, double b, double c) = (FuelType, Year < 2010) switch
        {
            (FuelType.Diesel, true) => (23, 18, 13),
            (FuelType.Petrol, true) => (18, 14, 10),
            (FuelType.Diesel, false) => (25, 20, 15),
            (FuelType.Petrol, false) => (20, 16, 12),
            _ => throw new ArgumentException(nameof(FuelType))
        };

        if (KmPerLiter >= a) 
        {
            return EnergyClass.A;
        }
        if (KmPerLiter >= b) 
        {
            return EnergyClass.B;
        }
        if (KmPerLiter >= c) 
        {
            return EnergyClass.C;
        }

        return EnergyClass.D;
    }

}


