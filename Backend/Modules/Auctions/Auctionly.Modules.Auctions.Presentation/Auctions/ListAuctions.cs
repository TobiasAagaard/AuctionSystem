using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Application.Auctions.ListAuctions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auctionly.Modules.Auctions.Presentation.Auctions;

public static class ListAuctions
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("/auctions", async (IQueryHandler<ListAuctionsQuery, List<ListAuctionsResponse>> handler, CancellationToken ct) =>
        {
            List<ListAuctionsResponse> auctions = await handler.Handle(new ListAuctionsQuery(), ct);

            return Results.Ok(auctions);
            
        }).WithTags(Tags.Auctions);
    }
}