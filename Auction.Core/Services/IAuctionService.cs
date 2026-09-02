using Auction_Core.Models;

namespace Auction_Core.Services;

public interface IAuctionService
{
    int SetForSale(Vehicle vehicle, ISeller seller, decimal minimumPrice);
    bool ReceiveBid(IBuyer buyer, int auctionId, decimal bidAmount);
    bool AcceptBid(ISeller seller, int auctionId);
}
