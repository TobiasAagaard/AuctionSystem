using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public static class GetAuctions
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auctions", async (IAuctionRepository repository, CancellationToken cancellationToken) =>
        {
            IEnumerable<Auction> auctions = await repository.GetAuctionsAsync(cancellationToken);
            return Results.Ok(new GetAuctionsResponse(auctions));
        })
        .WithTags("Auctions");
    }
}

public record GetAuctionsResponse(IEnumerable<Auction> Auctions);