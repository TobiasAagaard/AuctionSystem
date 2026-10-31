
namespace Auctionly.Modules.Auctions.Application.Auctions.SearchAuctions;

public sealed record SearchAuctionQuery(
    DateTime? StartTime,
    DateTime? EndTime,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? Make,
    string? Model,
    int? MinYear,
    int? MaxYear,
    int? MinMileage,
    int? MaxMileage,
    string? FuelType,
    string? VehicleType,
    int Page,
    int PageSize
);
