using Auction_Core.Enums;

namespace Auction_Core.Models;

public class SemiTruck : HeavyVehicle
{
    public SemiTruck(
        string name,
        double kilometers,
        string registrationNumber,
        int year,
        double basePrice,
        bool towBar,
        double engineSize,
        double kmPerLiter,
        double maxLoad,
        double height,
        double weight,
        double length
    ) : base(name, kilometers, registrationNumber, year, basePrice, towBar, towBar ? LicenseType.CE : LicenseType.C, engineSize, kmPerLiter, FuelType.Diesel, height, weight, length)
    {
        this.MaxLoad = maxLoad;

        if (engineSize < 4.2 || engineSize > 15)
        {
            throw new ArgumentOutOfRangeException(nameof(engineSize), "Engine size must be between 4.2 and 15 liters for a semi-truck.");
        }
    }

    public override LicenseType LicenseType => TowBar ? LicenseType.CE : LicenseType.C;
    public double MaxLoad  { get; private set; }
}