using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Application.Auctions.SearchAuctions;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Presentation.ApiResults;

namespace Auctionly.Modules.Auctions.Presentation.Auctions;

internal static class SearchAuctions
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auctions/search", async(
            IQueryHandler<SearchAuctionQuery, Result<SearchAuctionResponse>> handler,
            DateTime? startTime,
            DateTime? endTime,
            decimal? minPrice,
            decimal? maxPrice,
            int page = 1,
            int pageSize = 15,
            CancellationToken ct = default) =>
        {
            Result<SearchAuctionResponse> result = await handler.Handle(
                new SearchAuctionQuery(startTime, endTime, minPrice, maxPrice, page, pageSize),
                ct
            );

            return result.Match(Results.Ok, ApiResults.ApiResults.Problem);
        })
        .WithTags(Tags.Auctions);
    }
}