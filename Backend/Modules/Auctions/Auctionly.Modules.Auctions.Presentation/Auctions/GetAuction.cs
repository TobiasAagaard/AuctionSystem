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
        app.MapGet("/auctions/{id}", async (int id, IQueryHandler<GetAuctionQuery, GetAuctionResponse?> handler) =>
        {
            GetAuctionResponse? @event =await handler.Handle(new GetAuctionQuery(id), default);

            if (@event is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(@event);

        }).WithTags(Tags.Auctions);
    }
}