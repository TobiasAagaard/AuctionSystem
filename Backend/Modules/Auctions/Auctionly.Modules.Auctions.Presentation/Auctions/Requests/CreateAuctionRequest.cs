using Auctionly.Modules.Auctions.Domain.ValueObjects.Enums;

namespace Auctionly.Modules.Auctions.Presentation.Auctions.Requests;

public static partial class CreateAuctionRequest
{
    internal sealed record Request(
        string Title,
        string Description,
        decimal Price,
        Currency Currency,
        DateTime StartTime,
        DateTime EndTime,
        VehicleRequest Vehicle
    );
}