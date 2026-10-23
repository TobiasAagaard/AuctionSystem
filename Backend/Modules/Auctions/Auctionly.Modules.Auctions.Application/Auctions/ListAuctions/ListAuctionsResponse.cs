
namespace Auctionly.Modules.Auctions.Application.Auctions.ListAuctions;

public sealed record ListAuctionsResponse(
    int Id,
    string Title,
    decimal Price
);