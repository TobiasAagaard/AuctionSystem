using Auctionly.Modules.Auctions.Domain.ValueObjects.Enums;

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
    Currency Currency
);