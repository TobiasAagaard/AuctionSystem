namespace Auction_Core.Models;

public class Bid : IBid
{
    public Bid(int id, IBuyer buyer, IAuction auction, decimal amount, DateTime createdAt)
    {
        Id = id;
        Buyer = buyer;
        Auction = auction;
        Amount = amount;
        CreatedAt = createdAt;
    }
    public int Id { get; set; }
    public IAuction Auction { get; set; }
    public IBuyer Buyer { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}