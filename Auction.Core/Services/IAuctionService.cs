using Auction_Core.Models;

namespace Auction_Core.Services;

public interface IAuctionService
{
    Task<int> SetForSale(Vehicle vehicle, ISeller seller, decimal minimumPrice, DateTime endTime);
    Task<bool> ReceiveBid(IBuyer buyer, int auctionId, decimal bidAmount);
    Task<bool> AcceptBid(ISeller seller, int auctionId);
}
