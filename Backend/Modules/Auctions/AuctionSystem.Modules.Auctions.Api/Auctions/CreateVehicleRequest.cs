using System.Text.Json.Serialization;
using AuctionSystem.Modules.Auctions.Api.Auctions.Enums;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public static partial class CreateAuction
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(PersonalCarRequest), "PersonalCar")]
    [JsonDerivedType(typeof(BusinessCarRequest), "BusinessCar")]
    internal abstract record VehicleRequest(
        string Make,
        string Model,
        int Year,
        int Mileage,
        decimal BasePrice,
        double EngineSize,
        FuelType FuelType);

    internal sealed record PersonalCarRequest(
        string Make,
        string Model,
        int Year,
        int Mileage,
        decimal BasePrice,
        double EngineSize,
        FuelType FuelType,
        int SeatCount,
        bool Isofix)
        : VehicleRequest(Make, Model, Year, Mileage, BasePrice, EngineSize, FuelType);

    internal sealed record BusinessCarRequest(
        string Make,
        string Model,
        int Year,
        int Mileage,
        decimal BasePrice,
        double EngineSize,
        FuelType FuelType,
        double CargoVolume,
        double MaxPayload)
        : VehicleRequest(Make, Model, Year, Mileage, BasePrice, EngineSize, FuelType);
}
