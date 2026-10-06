using Auction_Core.Repository;

namespace Auction_Core.Models;

public class Auction
{
    public Auction(int id, User user, Vehicle vehicle, decimal minimumPrice, DateTime endTime)
        : this(id, user, vehicle, minimumPrice, false, endTime, DateTime.Now, DateTime.Now, null)
    {
    }

    public Auction(int id, User user, Vehicle vehicle, decimal minimumPrice, bool isSold, DateTime endTime, DateTime createdAt, DateTime updatedAt, NotificationDelegate? notificationFunction)
    {
        Id = id;
        Vehicle = vehicle;
        User = user;
        MinimumPrice = minimumPrice;
        IsSold = isSold;
        EndTime = endTime;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        NotificationFunction = notificationFunction ?? ((_, _) => { });
    }

    public int Id { get; set; }
    public Vehicle Vehicle { get; set; }
    public User User { get; set; }
    public decimal MinimumPrice { get; set; }
    public bool IsSold { get; set; } = false;
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public NotificationDelegate? NotificationFunction { get; }
}