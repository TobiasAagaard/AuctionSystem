using Auctionly.Modules.Auctions.Domain.Auctions;

namespace Auctionly.Modules.Auctions.Application.Auctions.PublishAuction;

public sealed record PublishAuctionCommand(
    int AuctionId,
    AuctionStatus Status
);