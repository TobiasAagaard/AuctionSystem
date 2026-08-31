namespace Auction_Core.Models;

public interface ISeller
{
    decimal Balance { get; set; }

    void ReceiveNotificationOfBid(Auction auction, decimal bid);
}
