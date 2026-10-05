using System.Reflection.Metadata;
using Auction_Core.Models;

namespace Auction_Core.Repository;

public interface IAuctionRepository
{
    Task<Auction> GetAuctionByIdAsync(int auctionId);
    Task<IEnumerable<Auction>> GetAllAuctionsAsync();
    Task<int> AddAuctionAsync(Vehicle vehicle, ISeller seller, decimal minimumPrice, DateTime endTime);
    Task<int> AddAuctionAsync(Vehicle vehicle, ISeller seller, decimal minimumPrice, DateTime endTime, NotificationDelegate notificationFunction);
    Task<bool> RemoveAuctionAsync(int auctionId);
    Task<bool> UpdateAuctionAsync(Auction auction);
    Task<bool> AddBidAsync(int auctionId, IBuyer buyer, decimal bidAmount);
    Task<IEnumerable<Bid>> GetBidsByAuctionIdAsync(int auctionId);
    Task<Bid?> GetHighestBidByAuctionIdAsync(int auctionId);
}