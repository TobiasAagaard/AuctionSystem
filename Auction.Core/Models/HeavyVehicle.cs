using Auction_Core.Enums;
namespace Auction_Core.Models;

public abstract class HeavyVehicle : Vehicle
{
    private double _weight;
    private double _height;
    private double _length;

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
        this._height = height;
        this._weight = weight;
        this._length = length;
    }
}