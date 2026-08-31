using Auction_Core.Models;

namespace Auction_Core.Services;

public class AuctionHouse
{
    public int SetForSale(Vehicle v, User s, decimal minPrice)
    {
        // Tænker at denne metode skal oprette en ny auktion og returnere dens ID. For nu, lad os bare kaste en NotImplementedException. Da vi ikke har en database til at gemme auktioner.
        var auction = new Auction(0, v, s, minPrice);
        throw new NotImplementedException("SetForSale method is not implemented yet.");
    }
}