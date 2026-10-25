namespace Auctionly.Modules.Auctions.Domain;

public class Bid
{
    public int Id { get; private set; }
    public int AuctionId { get; private set; }
    public decimal Amount { get; private set; }
}