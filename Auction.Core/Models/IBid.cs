namespace Auction_Core.Models;

public interface IBid
{
    int Id { get; set; }
    IAuction Auction { get; set; }
    IBuyer Buyer { get; set; }
    decimal Amount { get; set; }
    DateTime CreatedAt { get; set; }
}