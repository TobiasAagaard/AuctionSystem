using Auction_Core.Enums;

namespace Auction_Core.Models;

public class BusinessPersonalCar : PersonalCar
{
    public BusinessPersonalCar(
        int id,
        string name,
        double kilometers,
        string registrationNumber,
        int year,
        double basePrice,
        double engineSize,
        double kmPerLiter,
        FuelType fuelType,
        int seatCount,
        bool rollCage,
        double cargoCapacity
    ) : base(id, name, kilometers, registrationNumber, year, basePrice, true, engineSize, kmPerLiter, fuelType, seatCount, cargoCapacity > 750 ? LicenseType.BE : LicenseType.B)
    {
        if (seatCount != 2)
        {
            throw new ArgumentOutOfRangeException(nameof(seatCount), "A business personal car must have exactly 2 seats.");
        }

        this.RollCage = rollCage;
        this.CargoCapacity = cargoCapacity;
    }

    public override LicenseType LicenseType => CargoCapacity > 750 ? LicenseType.BE : LicenseType.B;
    public bool RollCage { get; set; }

    public double CargoCapacity { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, RollCage: {RollCage}, CargoCapacity: {CargoCapacity}";
    }
}
