using Auctionly.Modules.Auctions.Application.Auctions.GetAuction;

namespace Auctionly.Modules.Auctions.Application.Auctions.SearchAuctions;

public sealed record SearchAuctionResponse(
    int page,
    int pageSize,
    int TotalCount,
    IReadOnlyList<GetAuctionResponse> Auctions
);