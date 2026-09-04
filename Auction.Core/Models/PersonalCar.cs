using Auction_Core.Enums;

namespace Auction_Core.Models;

public abstract class PersonalCar : Vehicle
{

    protected PersonalCar(
        int id,
        string name,
        double kilometers,
        string registrationNumber,
        int year,
        double basePrice,
        bool towBar,
        double engineSize,
        double kmPerLiter,
        FuelType fuelType,
        int seatCount,
        LicenseType licenseType = LicenseType.B
    ) : base(id, name, kilometers, registrationNumber, year, basePrice, towBar, licenseType, engineSize, kmPerLiter, fuelType)
    {
        if (engineSize < 0.7 || engineSize > 10.0)
        {
            throw new ArgumentOutOfRangeException(nameof(engineSize), "Engine size must be between 0.7 and 10.0 liters for a personal car.");
        }

        if (licenseType is not (LicenseType.B or LicenseType.BE))
        {
            throw new ArgumentOutOfRangeException(nameof(licenseType), "A personal car requires either a B or BE license.");
        }

        this.SeatCount = seatCount;
    }

    public int SeatCount { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, SeatCount: {SeatCount}";
    }
}
