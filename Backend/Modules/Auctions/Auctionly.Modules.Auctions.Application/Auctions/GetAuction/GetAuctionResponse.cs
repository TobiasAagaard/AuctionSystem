using System.Text.Json.Serialization;
using Auctionly.Modules.Auctions.Domain.Vehicles;
using Auctionly.Modules.Auctions.Domain.ValueObjects.Enums;


namespace Auctionly.Modules.Auctions.Application.Auctions.GetAuction;

public sealed record GetAuctionResponse(
    int Id,
    string Title,
    string Description,
    decimal Price,
    Currency Currency,
    DateTime StartTime,
    DateTime EndTime,
    GetAuctionVehicleResponse Vehicle
    );

public sealed record GetAuctionVehicleResponse(
    int Id,
    string Make,
    string Model,
    int Year,
    int Mileage,
    decimal PurchasePrice,
    Currency Currency,
    double EngineSize,
    FuelType FuelType,
    VehicleSpecificationResponse Specification
);
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PersonalCarResponse))]
[JsonDerivedType(typeof(BusinessCarResponse))]
public abstract record VehicleSpecificationResponse;

public sealed record PersonalCarResponse(
    int SeatCount,
    bool Isofix
) : VehicleSpecificationResponse;
public sealed record BusinessCarResponse(
    double CargoVolume,
    double MaxPayload
) : VehicleSpecificationResponse;
