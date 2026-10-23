using Auctionly.Modules.Auctions.Application.Auctions.ListAuctions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auctionly.Modules.Auctions.Presentation.Auctions;

public static class ListAuctions
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/auctions", async (ISender sender) =>
        {
            List<ListAuctionsResponse> auctions = await sender.Send(new ListAuctionsQuery());

            return Results.Ok(auctions);
            
        }).WithTags(Tags.Auctions);
    }
}