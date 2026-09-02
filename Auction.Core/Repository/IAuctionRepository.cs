using Auction_Core.Models;

namespace Auction_Core.Repository;

public interface IAuctionRepository
{
    Auction GetAuctionById(int auctionId);
    IEnumerable<Auction> GetAllAuctions();
    bool AddAuction(Vehicle vehicle, ISeller seller, decimal minimumPrice);
    bool AddAuction(Vehicle vehicle, ISeller seller, decimal minimumPrice, NotificationDelegate notificationFunction);
    bool RemoveAuction(int auctionId);
    bool UpdateAuction(Auction auction);
}