using Auction_Core.Repository;

namespace Auction_Core.Models;

public class Auction
{
    public Auction(int id, Vehicle vehicle, ISeller seller, decimal minimumPrice)
        : this(id, vehicle, seller, minimumPrice, null)
    {
    }

    public Auction(int id, Vehicle vehicle, ISeller seller, decimal minimumPrice, NotificationDelegate? notificationFunction)
    {
        Id = id;
        Vehicle = vehicle;
        Seller = seller;
        MinimumPrice = minimumPrice;
        NotificationFunction = notificationFunction ?? ((_, _) => { });
    }

    public int Id { get; }
    public Vehicle Vehicle { get; }
    public ISeller Seller { get; }
    public decimal MinimumPrice { get; }
    public decimal HighestBid { get; set; } = 0;
    public IBuyer? HighestBidder { get; set; }
    public NotificationDelegate? NotificationFunction { get; }
}