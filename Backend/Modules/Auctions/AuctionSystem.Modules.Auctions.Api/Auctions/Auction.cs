namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public class Auction
{

    public Auction(string title, string description, decimal price, DateTime startTime, DateTime endTime)
    {
        Title = title;
        Description = description;
        Price = price;
        StartTime = startTime;
        EndTime = endTime;
        IsSold = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set;}
    public bool IsSold { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    
}