namespace Auction.Core.Models;

public class Auction
{
    public Auction(int id, Vehicle vehicle, Seller seller, decimal minimumPrice)
    {
        this.Id = id;
        this.Vehicle = vehicle;
        this.Seller = seller;
        this.MinimumPrice = minimumPrice;
    }
    public int Id { get; }
    public Vehicle Vehicle { get; }
    public Seller Seller { get; }
    public decimal MinimumPrice { get; }
}