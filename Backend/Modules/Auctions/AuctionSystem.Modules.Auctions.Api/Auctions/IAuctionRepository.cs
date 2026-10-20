namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public interface IAuctionRepository
{
    Task<int> AddAuctionAsync(Auction auction, CancellationToken cancellationToken);
    Task<Auction?> GetAuctionByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Auction>> GetAuctionsAsync(CancellationToken cancellationToken);
}
