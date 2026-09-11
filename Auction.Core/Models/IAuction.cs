namespace Auction_Core.Models;

public interface IAuction
{
    int Id { get; }
    ISeller Seller { get; }
    IVehicle Vehicle { get; }
    decimal MinimumPrice { get; }
    DateTime EndTime { get; }
    DateTime CreatedAt { get; }
    DateTime UpdatedAt { get; }
}