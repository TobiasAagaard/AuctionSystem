using AuctionSystem.Modules.Auctions.Api.Auctions.Enums;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;


public class Vehicle
{
    public Vehicle()
    {
        
    }
    public Vehicle(string make, string model, int year, int mileage, decimal basePrice, double engineSize, FuelType fuelType, VehicleDetails vehicleDetails)
    {

        if (fuelType == FuelType.Electric && engineSize != 0)
        {
            throw new ArgumentException("Electric vehicles should not have an engine size greater than zero.", nameof(engineSize));
        }
        else
        {
            if (fuelType != FuelType.Electric && engineSize <= 0)
            {
                throw new ArgumentException("Engine size must be greater than zero.", nameof(engineSize));
            }
        }

        if (year < 1886)
        {
            throw new ArgumentException("Year must be greater than or equal to 1886.", nameof(year));
        }

        Make = make;
        Model = model;
        Year = year;
        Mileage = mileage;
        BasePrice = basePrice;
        EngineSize = engineSize;
        FuelType = fuelType;
        VehicleDetails = vehicleDetails;
    }

    public int Id { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public int Mileage { get; private set; }
    public decimal BasePrice { get; private set; }
    public double EngineSize { get; private set; }
    public VehicleType VehicleType  => VehicleDetails switch
    {
        PersonalCar _ => VehicleType.PersonalCar,
        BusinessCar _ => VehicleType.BusinessCar,
        _ => throw new InvalidOperationException("Unknown vehicle type.")
    };
    public FuelType FuelType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public VehicleDetails VehicleDetails { get; private set; }
}

public abstract record VehicleDetails();

public record PersonalCar(
    int SeatCount,
    bool Isofix


) : VehicleDetails;

public record BusinessCar(
    double CargoVolume,
    double MaxPayload
) : VehicleDetails;
