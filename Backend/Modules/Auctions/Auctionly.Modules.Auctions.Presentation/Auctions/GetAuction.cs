using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Application.Auctions.GetAuction;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Presentation.ApiResults;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auctionly.Modules.Auctions.Presentation.Auctions;

internal static class GetAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auctions/{id}", async (int id, IQueryHandler<GetAuctionQuery, Result<GetAuctionResponse>> handler, CancellationToken ct) =>
        {
            Result<GetAuctionResponse> result = await handler.Handle(new GetAuctionQuery(id), ct);

            return result.Match(Results.Ok, ApiResults.ApiResults.Problem);
        })
        .WithTags(Tags.Auctions);
    }
}
