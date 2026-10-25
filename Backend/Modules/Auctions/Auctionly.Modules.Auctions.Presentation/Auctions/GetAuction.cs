using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

using Auctionly.Modules.Auctions.Application.Auctions.GetAuction;
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

namespace Auctionly.Modules.Auctions.Presentation.Auctions;

internal static class GetAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auctions/{id}", async (int id, IQueryHandler<GetAuctionQuery, GetAuctionResponse?> handler, CancellationToken ct) =>
        {
            GetAuctionResponse? auction =await handler.Handle(new GetAuctionQuery(id), ct);

            if (auction is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(auction);

        }).WithTags(Tags.Auctions);
    }
}