
using Auction_Core.Models;

namespace Auction_Core.Repository;

public interface IAuctionRepository
{
    Task<Auction?> GetAuctionByIdAsync(int auctionId);
    Task<IEnumerable<Auction>> GetAllAuctionsAsync();
    Task<int> AddAuctionAsync(Vehicle vehicle, User seller, decimal minimumPrice, DateTime endTime);
    Task<int> AddAuctionAsync(Vehicle vehicle, User seller, decimal minimumPrice, DateTime endTime, NotificationDelegate notificationFunction);
    Task DeleteAuctionAsync(int auctionId);
    Task<bool> UpdateAuctionAsync(Auction auction);
    Task<int> AddBidAsync(int auctionId, User buyer, decimal bidAmount);
    Task<IEnumerable<Bid>> GetBidsByAuctionIdAsync(int auctionId);
    Task<Bid?> GetHighestBidByAuctionIdAsync(int auctionId);
}