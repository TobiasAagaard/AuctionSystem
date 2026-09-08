namespace Auction_Core.Models;

public interface IBuyer
{
    int ID { get; }
    decimal Balance { get; set; }
}
