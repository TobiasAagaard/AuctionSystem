using Auction_Core.Enums;

namespace Auction_Core.Models;

public class Bus : HeavyVehicle
{
    public Bus(
        string name,
        double kilometers,
        string registrationNumber,
        int year,
        double basePrice,
        bool towBar,
        double engineSize,
        double kmPerLiter,
        double weight,
        double height,
        double length,
        int seats,
        int sleepingPlaces,
        bool hasToilet
    ) : base(name, kilometers, registrationNumber, year, basePrice, towBar, towBar ? LicenseType.DE : LicenseType.D, engineSize, kmPerLiter, FuelType.Diesel, height, weight, length)
    {
        Seats = seats;
        SleepingPlaces = sleepingPlaces;
        HasToilet = hasToilet;
    }

    public override LicenseType LicenseType => TowBar ? LicenseType.DE : LicenseType.D;
    public int Seats { get; set;}
    public int SleepingPlaces { get; set; }
    public bool HasToilet { get; set; }
    public override string ToString()
    {
        return $"Bus: {Name}, Kilometers: {Kilometers}, RegistrationNumber: {RegistrationNumber}, Year: {Year}, BasePrice: {BasePrice}, TowBar: {TowBar}, LicenseType: {LicenseType}, EngineSize: {EngineSize}, KmPerLiter: {KmPerLiter}, FuelType: {FuelType}, EnergyClass: {EnergyClass}, Seats: {Seats}, SleepingPlaces: {SleepingPlaces}, HasToilet: {HasToilet}";
    }
}