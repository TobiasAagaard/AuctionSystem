namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public interface IAuctionRepository
{
    Task AddAuctionAsync(Auction auction, CancellationToken cancellationToken);
    Task<Auction> GetAuctionByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Auction>> GetAuctionsAsync(CancellationToken cancellationToken);
}
