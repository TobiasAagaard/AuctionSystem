namespace Auctionly.Modules.Auctions.Domain.Bids;

public class Bid
{
    public int Id { get; private set; }
    public int AuctionId { get; private set; }
    public int BidderId { get; private set; }
    public decimal Amount { get; private set; }
}