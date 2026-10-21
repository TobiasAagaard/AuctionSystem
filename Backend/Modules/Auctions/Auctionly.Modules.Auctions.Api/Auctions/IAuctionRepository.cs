namespace Auctionly.Modules.Auctions.Api.Auctions;

public interface IAuctionRepository
{
    Task<int> AddAuctionAsync(Auction auction, CancellationToken cancellationToken);
    Task<GetAuctionResponse?> GetAuctionByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<GetAuctionResponse>> GetAuctionsAsync(CancellationToken cancellationToken);
}
