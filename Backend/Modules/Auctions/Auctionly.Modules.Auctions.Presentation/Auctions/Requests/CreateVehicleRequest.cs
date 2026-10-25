using System.Text.Json.Serialization;
using Auctionly.Modules.Auctions.Domain.Vehicles;
using Auctionly.Modules.Auctions.Domain.ValueObjects.Enums;

namespace Auctionly.Modules.Auctions.Presentation.Auctions.Requests;

public static partial class CreateAuctionRequest
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(PersonalCarRequest), "PersonalCar")]
    [JsonDerivedType(typeof(BusinessCarRequest), "BusinessCar")]
    internal abstract record VehicleRequest(
        string Make,
        string Model,
        int Year,
        int Mileage,
        decimal PurchasePrice,
        Currency Currency,
        double EngineSize,
        FuelType FuelType);

    internal sealed record PersonalCarRequest(
        string Make,
        string Model,
        int Year,
        int Mileage,
        decimal PurchasePrice,
        Currency Currency,
        double EngineSize,
        FuelType FuelType,
        int SeatCount,
        bool Isofix)
        : VehicleRequest(Make, Model, Year, Mileage, PurchasePrice, Currency, EngineSize, FuelType);

    internal sealed record BusinessCarRequest(
        string Make,
        string Model,
        int Year,
        int Mileage,
        decimal PurchasePrice,
        Currency Currency,
        double EngineSize,
        FuelType FuelType,
        double CargoVolume,
        double MaxPayload)
        : VehicleRequest(Make, Model, Year, Mileage, PurchasePrice, Currency, EngineSize, FuelType);
}