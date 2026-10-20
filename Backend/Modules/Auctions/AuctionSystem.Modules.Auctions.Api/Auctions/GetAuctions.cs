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
            IReadOnlyList<GetAuctionResponse> auctions = await repository.GetAuctionsAsync(cancellationToken);
            return Results.Ok(new GetAuctionsResponse(auctions.Select(@auction => new AuctionSummary(
                @auction.Id,
                @auction.Title,
                @auction.Price,
                @auction.Status,
                @auction.StartTime,
                @auction.EndTime,
                @auction.Vehicle
            )).ToList()));
        })
        .WithTags("Auctions");
    }
}

public record AuctionSummary(
    int Id,
    string Title,
    decimal Price,
    AuctionStatus Status,
    DateTime StartTime,
    DateTime? EndTime,
    VehicleResponse Vehicle
);

public record GetAuctionsResponse(
    IReadOnlyList<AuctionSummary> Auctions
);