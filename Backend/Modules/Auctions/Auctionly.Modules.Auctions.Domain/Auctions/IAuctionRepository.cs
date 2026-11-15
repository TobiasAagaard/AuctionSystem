using Auctionly.Modules.Auctions.Domain.Abstractions;

namespace Auctionly.Modules.Auctions.Domain.Auctions;

public interface IAuctionRepository
{
    Task<int> AddAuctionAsync(Auction auction, CancellationToken cancellationToken = default);
    Task<Result<int>> PublishAuctionAsync(int auctionId, CancellationToken cancellationToken = default);
}
