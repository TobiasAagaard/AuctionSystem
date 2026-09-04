using Auction_Core.Enums;

namespace Auction_Core.Models;

public class PrivatePersonalCar : PersonalCar
{
    public PrivatePersonalCar(
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
        bool isofix,
        LicenseType licenseType = LicenseType.B
    ) : base(id, name, kilometers, registrationNumber, year, basePrice, towBar, engineSize, kmPerLiter, fuelType, seatCount, licenseType)
    {
        if (seatCount < 2 || seatCount > 7)
        {
            throw new ArgumentOutOfRangeException(nameof(seatCount), "A private personal car must have between 2 and 7 seats.");
        }

        this.Isofix = isofix;
    }

    public bool Isofix { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, Isofix: {Isofix}";
    }
}