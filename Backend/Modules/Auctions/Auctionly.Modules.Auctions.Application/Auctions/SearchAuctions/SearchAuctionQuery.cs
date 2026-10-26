
namespace Auctionly.Modules.Auctions.Application.Auctions.SearchAuctions;

public sealed record SearchAuctionQuery(
    DateTime? StartTime,
    DateTime? EndTime,
    decimal? MinPrice,
    decimal? MaxPrice,
    int Page,
    int PageSize
);