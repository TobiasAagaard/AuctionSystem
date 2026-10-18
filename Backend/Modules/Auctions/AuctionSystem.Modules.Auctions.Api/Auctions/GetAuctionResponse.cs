namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public record GetAuctionResponse(
    int Id,
    string Title,
    string Description,
    decimal Price,
    DateTime StartTime,
    DateTime? EndTime,
    bool IsSold
);
