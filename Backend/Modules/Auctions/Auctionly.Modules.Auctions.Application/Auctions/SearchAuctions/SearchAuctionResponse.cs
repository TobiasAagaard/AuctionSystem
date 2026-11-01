using Auctionly.Modules.Auctions.Domain.ValueObjects;
using Auctionly.Modules.Auctions.Domain.ValueObjects.Enums;
using Auctionly.Modules.Auctions.Domain.Vehicles;

namespace Auctionly.Modules.Auctions.Application.Auctions.SearchAuctions;

public sealed record SearchAuctionResponse(
    int Page,
    int PageSize,
    int TotalCount,
    IReadOnlyList<SearchAuctionItem> Auctions
);

public sealed record SearchAuctionItem(
    int Id,
    string Title,
    string Description,
    DateTime StartTime,
    DateTime EndTime,
    decimal Price,
    Currency Currency,
    SearchAuctionVehicle Vehicle
);

public sealed record SearchAuctionVehicle(
    string Make,
    string Model,
    int Year,
    int Mileage,
    double EngineSize,
    FuelType FuelType,
    VehicleType VehicleType);