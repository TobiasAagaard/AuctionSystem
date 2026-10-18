using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public static class GetAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auctions/{id}", async (int id, AuctionRepository auctionRepository, CancellationToken cancellationToken) =>
        {
            AuctionResponse? @auction = await auctionRepository.GetAuctionByIdAsync(id, cancellationToken) is Auction auctionEntity
                ? new AuctionResponse(
                    auctionEntity.Id,
                    auctionEntity.Title,
                    auctionEntity.Description,
                    auctionEntity.Price,
                    auctionEntity.StartTime,
                    auctionEntity.EndTime,
                    auctionEntity.IsSold
                )
                : null;

            return @auction is not null ? Results.Ok(@auction) : Results.NotFound();
        })
        .WithTags(Tags.Auctions);

    }
}

public record AuctionResponse(
    int Id,
    string Title,
    string Description,
    decimal Price,
    DateTime StartTime,
    DateTime? EndTime,
    bool IsSold
);