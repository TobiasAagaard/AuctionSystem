using Auction_Core.Models;

namespace Auction_Core.Services;

public interface IAuctionHouse
{
    IReadOnlyList<Auction> SoldAuctions { get; }

    int SetForSale(Vehicle vehicle, ISeller seller, decimal minimumPrice);
    int SetForSale(Vehicle vehicle, ISeller seller, decimal minimumPrice, NotificationDelegate notificationFunction);
    bool ReceiveBid(IBuyer buyer, int auctionId, decimal bidAmount);
    bool AcceptBid(ISeller seller, int auctionId);
}
