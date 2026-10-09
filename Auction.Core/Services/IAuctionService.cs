using Auction_Core.Models;

namespace Auction_Core.Services;

public interface IAuctionService
{
    Task<int> SetForSale(Vehicle vehicle, User seller, decimal minimumPrice, DateTime endTime);
    Task<bool> ReceiveBid(User buyer, int auctionId, decimal bidAmount);
    Task<bool> AcceptBid(User seller, int auctionId);
}