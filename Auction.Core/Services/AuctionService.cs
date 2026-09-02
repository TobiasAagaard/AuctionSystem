using Auction_Core.Models;
using Auction_Core.Repository;

namespace Auction_Core.Services;



public class AuctionService : IAuctionService
{
    private readonly IAuctionRepository _auctionRepository;
    private readonly List<Auction> _soldAuctions = new List<Auction>();

    public AuctionService(IAuctionRepository auctionRepository)
    {
        _auctionRepository = auctionRepository ?? throw new ArgumentNullException(nameof(auctionRepository), "Auction repository cannot be null.");
    }

    public IReadOnlyList<Auction> SoldAuctions => _soldAuctions;

    public int SetForSale(Vehicle vehicle, ISeller seller, decimal minimumPrice)
    {
        if (seller == null) throw new ArgumentNullException(nameof(seller), "Seller cannot be null.");


        return SetForSale(vehicle, seller, minimumPrice, (auction, bid) => seller.ReceiveNotificationOfBid(auction, bid));
    }

    public int SetForSale(Vehicle vehicle, ISeller seller, decimal minimumPrice, NotificationDelegate notificationFunction)
    {
        if (vehicle == null) throw new ArgumentNullException(nameof(vehicle), "Vehicle cannot be null.");
        if (seller == null) throw new ArgumentNullException(nameof(seller), "Seller cannot be null.");
        if (notificationFunction == null) throw new ArgumentNullException(nameof(notificationFunction), "Notification function cannot be null.");
        if (minimumPrice < 0) throw new ArgumentOutOfRangeException(nameof(minimumPrice), "Minimum price cannot be negative.");

        _auctionRepository.AddAuction(vehicle, seller, minimumPrice, notificationFunction);
        return 0; 

    }

    public bool ReceiveBid(IBuyer buyer, int auctionId, decimal bidAmount)
    {
        if (buyer == null) throw new ArgumentNullException(nameof(buyer), "Buyer cannot be null.");
        if (bidAmount < 0) throw new ArgumentOutOfRangeException(nameof(bidAmount), "Bid cannot be negative.");

        var auction = _auctionRepository.GetAuctionById(auctionId);
        if (auction == null) return false;

        if (bidAmount <= auction.HighestBid) return false;
        if (buyer.Balance < bidAmount) return false;

        auction.HighestBid = bidAmount;
        auction.HighestBidder = buyer;

        if (bidAmount >= auction.MinimumPrice)
        {
            auction.NotificationFunction?.Invoke(auction, bidAmount);
        }

        return true;
    }

    public bool AcceptBid(ISeller seller, int auctionId)
    {
        if (seller == null) throw new ArgumentNullException(nameof(seller), "Seller cannot be null.");

        var auction = _auctionRepository.GetAuctionById(auctionId);
        if (auction == null) return false;

        if (!ReferenceEquals(auction.Seller, seller)) return false;

        if (auction.HighestBidder == null) return false;
        if (auction.HighestBid < auction.MinimumPrice) return false;

        if (auction.HighestBidder.Balance < auction.HighestBid) return false;

        auction.HighestBidder.Balance -= auction.HighestBid;
        seller.Balance += auction.HighestBid;

        _auctionRepository.RemoveAuction(auction.Id);
        _soldAuctions.Add(auction);

        return true;
    }
}
