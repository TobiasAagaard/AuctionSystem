namespace Auction_Core.Models;

public interface ISeller
{
    int ID { get; }
    decimal Balance { get; set; }

    void ReceiveNotificationOfBid(IAuction auction, decimal bid);
}
