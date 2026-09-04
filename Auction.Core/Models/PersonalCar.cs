using Auction_Core.Enums;

namespace Auction_Core.Models;

public abstract class PersonalCar : Vehicle
{

    public PersonalCar(
        int id,
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
        int seatCount
    ) : base(id, name, kilometers, registrationNumber, year, basePrice, towBar, licenseType, engineSize, kmPerLiter, fuelType)
    {
        this.SeatCount = seatCount;
    }

    public int SeatCount { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, SeatCount: {SeatCount}";
    }
}
