using System.Reflection.Metadata;
using Auction_Core.Models;

namespace Auction_Core.Repository;

public interface IAuctionRepository
{
    Task<Auction> GetAuctionByIdAsync(int auctionId);
    Task<IEnumerable<Auction>> GetAllAuctionsAsync();
    Task<bool> AddAuctionAsync(Vehicle vehicle, User seller, decimal minimumPrice);
    Task<bool> AddAuctionAsync(Vehicle vehicle, User seller, decimal minimumPrice, NotificationDelegate notificationFunction);
    Task<bool> RemoveAuctionAsync(int auctionId);
    Task<bool> UpdateAuctionAsync(Auction auction);
}