using Auctionly.Modules.Auctions.Domain.Vehicles;

namespace Auctionly.Modules.Auctions.Domain.Auctions;



public class Auction
{
    public Auction() { }
    public Auction(string title, string description, decimal price, DateTime startTime, DateTime endTime, Vehicle vehicle)
    {
        Title = title;
        Description = description;
        Price = price;
        StartTime = startTime;
        EndTime = endTime;
        Status = AuctionStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsDeleted = false;
        Vehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
    }

    public int Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public AuctionStatus Status { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public Vehicle Vehicle { get; private set; }
}
