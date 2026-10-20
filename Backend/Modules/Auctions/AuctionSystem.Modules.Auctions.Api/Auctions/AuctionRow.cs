using AuctionSystem.Modules.Auctions.Api.Auctions.Enums;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;

// One flat row from the auctions + vehicles + detail tables join.
// Detail columns are nullable because only one detail table matches per vehicle.
internal sealed class AuctionRow
{
    // Auction
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public AuctionStatus Status { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }

    // Vehicle
    public int VehicleId { get; init; }
    public string Make { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public int Year { get; init; }
    public int Mileage { get; init; }
    public decimal BasePrice { get; init; }
    public double EngineSize { get; init; }
    public FuelType FuelType { get; init; }
    public VehicleType VehicleType { get; init; }

    // PersonalCar (NULL for business cars)
    public int? SeatCount { get; init; }
    public bool? Isofix { get; init; }

    // BusinessCar (NULL for personal cars)
    public double? CargoVolume { get; init; }
    public double? MaxPayload { get; init; }
}
