using Auction_Core.Models;
using Auction_Core.Repository;

namespace Auction_Core.Services;

public class AuctionService : IAuctionService
{
    private readonly IAuctionRepository _auctionRepository;

    public AuctionService(IAuctionRepository auctionRepository)
    {
        _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository), "Auction repository cannot be null.");
    }

    public async Task<int> SetForSale(Vehicle vehicle, ISeller seller, decimal minimumPrice, DateTime endTime)
    {
        if (seller == null) throw new ArgumentNullException(nameof(seller), "Seller cannot be null.");
        if (vehicle == null) throw new ArgumentNullException(nameof(vehicle), "Vehicle cannot be null.");
        if (minimumPrice < 0) throw new ArgumentOutOfRangeException(nameof(minimumPrice), "Minimum price cannot be negative.");
        if (endTime < DateTime.UtcNow) throw new ArgumentOutOfRangeException(nameof(endTime), "End time cannot be in the past.");

        return await SetForSale(vehicle, seller, minimumPrice, endTime, seller.ReceiveNotificationOfBid);
    }

    public async Task<int> SetForSale(Vehicle vehicle, ISeller seller, decimal minimumPrice, DateTime endTime, NotificationDelegate notificationFunction)
    {
        if (vehicle == null) throw new ArgumentNullException(nameof(vehicle), "Vehicle cannot be null.");
        if (seller == null) throw new ArgumentNullException(nameof(seller), "Seller cannot be null.");
        if (minimumPrice < 0) throw new ArgumentOutOfRangeException(nameof(minimumPrice), "Minimum price cannot be negative.");
        if (notificationFunction == null) throw new ArgumentNullException(nameof(notificationFunction), "Notification function cannot be null.");
        if (endTime < DateTime.UtcNow) throw new ArgumentOutOfRangeException(nameof(endTime), "End time cannot be in the past.");

        
        return await _auctionRepository.AddAuctionAsync(vehicle, seller, minimumPrice, endTime, notificationFunction);
    }

    public async Task<bool> ReceiveBid(IBuyer buyer, int auctionId, decimal bidAmount)
    {
        if (buyer == null) throw new ArgumentNullException(nameof(buyer), "Buyer cannot be null.");
        if (bidAmount < 0) throw new ArgumentOutOfRangeException(nameof(bidAmount), "Bid cannot be negative.");

        var auction = await _auctionRepository.GetAuctionByIdAsync(auctionId);
        if (auction == null) return false;

        var highestBid = await _auctionRepository.GetHighestBidByAuctionIdAsync(auctionId);
        if (highestBid == null) return false;
        if (bidAmount <= highestBid.Amount) return false;
        if (buyer.Balance < bidAmount) return false;

        if (bidAmount >= auction.MinimumPrice)
        {
            auction.NotificationFunction?.Invoke(auction, bidAmount);
        }

        return await _auctionRepository.AddBidAsync(auctionId, buyer, bidAmount);
    }

    public async Task<bool> AcceptBid(ISeller seller, int auctionId)
    {
        if (seller == null) throw new ArgumentNullException(nameof(seller), "Seller cannot be null.");

        

        var auction = await _auctionRepository.GetAuctionByIdAsync(auctionId) ?? throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");
        if (!ReferenceEquals(auction.Seller, seller)) return false;
        if (DateTime.UtcNow >= auction.EndTime)
        {
            return false;
        }

        var highestBid = await _auctionRepository.GetHighestBidByAuctionIdAsync(auctionId);
        if (highestBid == null) return false;
        if (highestBid.Amount < auction.MinimumPrice) return false;

        if (highestBid.Buyer.Balance < highestBid.Amount) return false;

        highestBid.Buyer.Balance -= highestBid.Amount;
        seller.Balance += highestBid.Amount;

        await _auctionRepository.RemoveAuctionAsync(auction.Id);

        return true;
    }
}