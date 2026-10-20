using System.Text.Json.Serialization;
using AuctionSystem.Modules.Auctions.Api.Auctions.Enums;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public record GetAuctionResponse(
    int Id,
    string Title,
    string Description,
    decimal Price,
    AuctionStatus Status,
    DateTime StartTime,
    DateTime? EndTime,
    VehicleResponse Vehicle
);

public record VehicleResponse(
    int Id,
    string Make,
    string Model,
    int Year,
    int Mileage,
    decimal BasePrice,
    double EngineSize,
    FuelType FuelType,
    VehicleDetailsResponse specifications
);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PersonalCarResponse), "PersonalCar")]
[JsonDerivedType(typeof(BusinessCarResponse), "BusinessCar")]
public abstract record VehicleDetailsResponse;

public record PersonalCarResponse(int SeatCount, bool Isofix) : VehicleDetailsResponse;

public record BusinessCarResponse(double CargoVolume, double MaxPayload) : VehicleDetailsResponse;
