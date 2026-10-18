using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public static class GetAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auctions/{id}", async (int id, [FromServices] IAuctionRepository auctionRepository, CancellationToken cancellationToken) =>
        {
            GetAuctionResponse? @auction = await auctionRepository.GetAuctionByIdAsync(id, cancellationToken) is Auction auctionEntity
                ? new GetAuctionResponse(
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
