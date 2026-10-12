namespace Auction_Core.Models;

public class Bid
{
    public Bid(int id, User user, Auction auction, decimal amount, DateTime createdAt)
    {
        Id = id;
        User = user;
        Auction = auction;
        Amount = amount;
        CreatedAt = createdAt;
    }
    public int Id { get; set; }
    public Auction Auction { get; set; }
    public User User { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}