namespace Auctionly.Modules.Auctions.Domain.Auctions;

// The write side's view of storage. Implemented in Infrastructure with Dapper.
public interface IAuctionRepository
{
    Task<int> AddAsync(Auction auction, CancellationToken cancellationToken = default);
}
