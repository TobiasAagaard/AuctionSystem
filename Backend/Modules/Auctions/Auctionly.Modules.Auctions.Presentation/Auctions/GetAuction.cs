using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MediatR;
using Auctionly.Modules.Auctions.Application.Auctions.GetAuction;

namespace Auctionly.Modules.Auctions.Presentation.Auctions;

internal static class GetAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auctions/{id}", async (int id, ISender sender) =>
        {
            GetAuctionResponse? @event =await sender.Send(new GetAuctionQuery(id));

            if (@event is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(@event);

        }).WithTags(Tags.Auctions);
    }
}