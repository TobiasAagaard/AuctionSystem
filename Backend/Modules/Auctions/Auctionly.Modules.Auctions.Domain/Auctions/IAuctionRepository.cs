namespace Auctionly.Modules.Auctions.Domain.Auctions;

public interface IAuctionRepository
{
    Task<int> AddAuctionAsync(Auction auction, CancellationToken cancellationToken = default);
    Task<int> PublishAuctionAsync(Auction auction, CancellationToken cancellationToken = default);
}
