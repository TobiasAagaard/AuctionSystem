using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public static partial class CreateAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auctions", async (CreateAuctionRequest request, IAuctionRepository auctionRepository, CancellationToken cancellationToken) =>
        {
            var @auction = new Auction(
                request.Title,
                request.Description,
                request.Price,
                request.StartTime,
                request.EndTime
            );

            int auctionId = await auctionRepository.AddAuctionAsync(@auction, cancellationToken);

            return auctionId != 0 ? Results.Created($"/auctions/{auctionId}", new
            {
                Id = auctionId
            }) : Results.BadRequest();

        })
        .WithTags(Tags.Auctions);
    }
}