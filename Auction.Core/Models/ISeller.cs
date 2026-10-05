namespace Auction_Core.Models;

public interface ISeller
{
    int ID { get; }
    decimal Balance { get; set; }

    void ReceiveNotificationOfBid(Auction auction, decimal bid);
}
