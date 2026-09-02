using Auction_Core.Models;

namespace Auction_Core.Repository;

public delegate void NotificationDelegate(Auction auction, decimal bid);

public class AuctionRepository : IAuctionRepository
{

    private IEnumerable<Auction> _auctions = new List<Auction>();
    private int _nextId = 1;


    public bool AddAuction(Vehicle vehicle, ISeller seller, decimal minimumPrice, NotificationDelegate? notificationFunction)
    {
        var newAuction = new Auction(
            _nextId++, 
            vehicle, 
            seller, 
            minimumPrice,
            notificationFunction
        );
        _auctions = _auctions.Append(newAuction);
        return true;
    }

    public bool AddAuction(Vehicle vehicle, ISeller seller, decimal minimumPrice)
    {
        return AddAuction(vehicle, seller, minimumPrice, null);
    }

    public IEnumerable<Auction> GetAllAuctions()
    {
        if (_auctions == null)
        {
            return new List<Auction>();
        }

        return _auctions;
    }

    public Auction GetAuctionById(int auctionId)
    {
        var auction = _auctions.FirstOrDefault(a => a.Id == auctionId);
        if (auction == null)
        {
            throw new KeyNotFoundException("This auction does not exist.");
        }
        return auction;
    }

    public bool RemoveAuction(int auctionId)
    {
        throw new NotImplementedException();
    }

    public bool UpdateAuction(Auction auction)
    {
        throw new NotImplementedException();
    }
}
