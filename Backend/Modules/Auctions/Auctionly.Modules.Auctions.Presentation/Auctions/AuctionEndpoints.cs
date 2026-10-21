using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Auctionly.Modules.Auctions.Presentation.Auctions;

public static class AuctionEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        CreateAuction.MapEndpoint(app);

        app.MapGet("/auctions", () => "Get all auctions");
    }
}
