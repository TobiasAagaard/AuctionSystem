using Auction_Core.Models;
using Auction_Core.Repository;

namespace Auction_Test;

internal sealed class TestSeller : ISeller
{
    public decimal Balance { get; set; }
    public List<(Auction Auction, decimal Bid)> Notifications { get; } = new();

    public void ReceiveNotificationOfBid(Auction auction, decimal bid)
    {
        Notifications.Add((auction, bid));
    }
}

internal sealed class TestBuyer : IBuyer
{
    public decimal Balance { get; set; }
}

internal sealed class FakeAuctionRepository : IAuctionRepository
{
    private readonly Dictionary<int, Auction> _auctions = new();
    private int _nextId = 1;

    public Auction GetAuctionById(int auctionId)
    {
        return _auctions.TryGetValue(auctionId, out Auction? auction) ? auction : null!;
    }

    public IEnumerable<Auction> GetAllAuctions() => _auctions.Values;

    public bool AddAuction(Vehicle vehicle, ISeller seller, decimal minimumPrice)
    {
        return AddAuction(vehicle, seller, minimumPrice, (_, _) => { });
    }

    public bool AddAuction(Vehicle vehicle, ISeller seller, decimal minimumPrice, NotificationDelegate notificationFunction)
    {
        var auction = new Auction(_nextId, vehicle, seller, minimumPrice, notificationFunction);
        _auctions.Add(_nextId, auction);
        _nextId++;
        return true;
    }

    public bool RemoveAuction(int auctionId)
    {
        return _auctions.Remove(auctionId);
    }

    public bool UpdateAuction(Auction auction)
    {
        if (!_auctions.ContainsKey(auction.Id)) return false;

        _auctions[auction.Id] = auction;
        return true;
    }
}
