
namespace Auctionly.Modules.Auctions.Presentation.Auctions.Requests;

public static partial class CreateAuctionRequest
{
    internal sealed record Request(
        string Title,
        string Description,
        decimal StartingPrice,
        DateTime StartTime,
        DateTime EndTime,
        VehicleRequest Vehicle
    );
}