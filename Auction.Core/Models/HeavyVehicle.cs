using Auction_Core.Enums;
namespace Auction_Core.Models;

public abstract class HeavyVehicle : Vehicle
{

    protected HeavyVehicle(
        string name, 
        double kilometers, 
        string registrationNumber, 
        int year, 
        double basePrice, 
        bool towBar, 
        LicenseType licenseType,
        double engineSize,
        double kmPerLiter,
        FuelType fuelType,
        double height,
        double weight,
        double length) : base(name, kilometers, registrationNumber, year, basePrice, towBar, licenseType, engineSize, kmPerLiter, fuelType)
    {
        this.Height = height;
        this.Weight = weight;
        this.Length = length;
    }

    public double Height { get; set; }
    public double Weight { get; set; }
    public double Length { get; set; }

    public override string ToString()
    {
        return $"{Name} - {Year} - {Kilometers} km - {BasePrice} kr - {Height} m - {Weight} kg - {Length} m";
    }
}