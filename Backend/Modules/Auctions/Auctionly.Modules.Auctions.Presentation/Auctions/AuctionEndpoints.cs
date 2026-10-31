using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Auctionly.Modules.Auctions.Presentation.Auctions;

public static class AuctionEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        CreateAuction.MapEndpoint(app);
        GetAuction.MapEndpoint(app);
        ListAuctions.MapEndpoints(app);
        SearchAuctions.MapEndpoint(app);
    }
}
