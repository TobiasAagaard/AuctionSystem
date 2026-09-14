using System.Reflection.Metadata;
using Auction_Core.Models;

namespace Auction_Core.Repository;

public interface IAuctionRepository
{
    Task<IAuction> GetAuctionByIdAsync(int auctionId);
    Task<IEnumerable<IAuction>> GetAllAuctionsAsync();
    Task<int> AddAuctionAsync(IVehicle vehicle, ISeller seller, decimal minimumPrice, DateTime endTime);
    Task<int> AddAuctionAsync(IVehicle vehicle, ISeller seller, decimal minimumPrice, DateTime endTime, NotificationDelegate notificationFunction);
    Task<bool> RemoveAuctionAsync(int auctionId);
    Task<bool> UpdateAuctionAsync(IAuction auction);
    Task<bool> AddBidAsync(int auctionId, IBuyer buyer, decimal bidAmount);
    Task<IEnumerable<IBid>> GetBidsByAuctionIdAsync(int auctionId);
    Task<IBid?> GetHighestBidByAuctionIdAsync(int auctionId);
}