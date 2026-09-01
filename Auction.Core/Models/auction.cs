using Auction_Core.Services;

namespace Auction_Core.Models;

public class Auction
{
    public Auction(int id, Vehicle vehicle, User seller, decimal minimumPrice, NotificationDelegate notificationFunction)
    {
        this.Id = id;
        this.Vehicle = vehicle;
        this.Seller = seller;
        this.MinimumPrice = minimumPrice;
        this.NotificationFunction = notificationFunction;
    }
    public int Id { get; }
    public Vehicle Vehicle { get; }
    public User Seller { get; }
    public decimal MinimumPrice { get; }
    public decimal HighestBid { get; set; } = 0;
    public NotificationDelegate NotificationFunction { get; set; }
}