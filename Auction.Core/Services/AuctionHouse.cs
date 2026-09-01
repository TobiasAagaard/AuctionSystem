using Auction_Core.Models;

namespace Auction_Core.Services;

// Delegate der matcher signaturen for en notifikationsmetode
public delegate void NotificationDelegate(Auction auction, decimal bid);

public class AuctionHouse
{
    private Dictionary<int, Auction> _auctions = new Dictionary<int, Auction>();
    private int _næsteAuktionsId = 1;

    // A3: Standard version (Calls the overloaded method with a standard delegate, that directly calls the notification method on the seller)
    public int SetForSale(Vehicle v, User s, decimal minPrice)
    {
        // We create a lambda/anonymous method that matches the NotificationDelegate signature and calls the ReceiveNotificationOfBid method on the seller.
        NotificationDelegate defaultNotification = (auction, bid) => s.ReceiveNotificationOfBid(auction, bid);

        return SetForSale(v, s, minPrice, defaultNotification);
    }

    // A4: Overload method that takes a NotificationDelegate as a parameter
    public int SetForSale(Vehicle v, User s, decimal minPrice, NotificationDelegate notificationFunction)
    {
        if (v == null) throw new ArgumentNullException(nameof(v), "Vehicle cannot be null.");
        if (s == null) throw new ArgumentNullException(nameof(s), "Seller cannot be null.");
        if (minPrice < 0) throw new ArgumentOutOfRangeException(nameof(minPrice), "Minimum price cannot be negative.");
        if (notificationFunction == null) throw new ArgumentNullException(nameof(notificationFunction), "Notification function cannot be null.");

        int auctionId = _næsteAuktionsId++;

        _auctions[auctionId] = new Auction
        (
            auctionId,
            v,
            s,
            minPrice,
            notificationFunction
        );

        return auctionId;
    }

    public bool RecieveBid(IBuyer buyer, int auctionId, decimal bid)
    {
        if (buyer == null) throw new ArgumentNullException(nameof(buyer), "Buyer cannot be null.");
        if (bid < 0) throw new ArgumentOutOfRangeException(nameof(bid), "Bid cannot be negative.");

        if (!_auctions.TryGetValue(auctionId, out var auction)) return false;

        if (bid <= auction.HighestBid) return false; // Bid must be higher than the current highest bid

        if (buyer.Balance < bid) return false; // Buyer must have enough balance to place the bid

        auction.HighestBid = bid; // Update the highest bid

        auction.NotificationFunction?.Invoke(auction, bid);

        return true; // Bid accepted
    }
}