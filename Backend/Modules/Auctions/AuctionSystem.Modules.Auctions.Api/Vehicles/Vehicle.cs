using AuctionSystem.Modules.Auctions.Api.Auctions;

namespace AuctionSystem.Modules.Auctions.Api.Vehicles;


public class Vehicle
{
    public int Id { get; private set; }
    public string Make { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public int Mileage { get; private set; }
    public decimal BasePrice { get; private set; }
    public double EngineSize { get; private set; }
    public FuelType FuelType { get; private set; }
    public VehicleType VehicleType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Auction Auction { get; private set; } = null!;
}

public enum FuelType
{
    Petrol,
    Diesel,
    Electric,
    Hybrid
}

public enum VehicleType
{
    PersonalCar,
    BusinessCar
}