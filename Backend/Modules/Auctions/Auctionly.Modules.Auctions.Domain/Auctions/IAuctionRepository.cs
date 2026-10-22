namespace Auctionly.Modules.Auctions.Domain.Auctions;

public interface IAuctionRepository
{
    Task<int> AddAuctionAsync(Auction auction, CancellationToken cancellationToken = default);
}
