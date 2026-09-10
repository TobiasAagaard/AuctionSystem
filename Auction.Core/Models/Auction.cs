using Auction_Core.Repository;

namespace Auction_Core.Models;

public class Auction
{
    public Auction(int id, ISeller seller, Vehicle vehicle, decimal minimumPrice, DateTime endTime)
        : this(id, seller, vehicle, minimumPrice, endTime, DateTime.Now, DateTime.Now, null)
    {
    }

    public Auction(int id, ISeller seller, Vehicle vehicle, decimal minimumPrice, DateTime endTime, DateTime createdAt, DateTime updatedAt, NotificationDelegate? notificationFunction)
    {
        Id = id;
        Vehicle = vehicle;
        Seller = seller;
        MinimumPrice = minimumPrice;
        EndTime = endTime;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        NotificationFunction = notificationFunction ?? ((_, _) => { });
    }

    public int Id { get; set; }
    public Vehicle Vehicle { get; set; }
    public ISeller Seller { get; set; }
    public decimal MinimumPrice { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public NotificationDelegate? NotificationFunction { get; }
}