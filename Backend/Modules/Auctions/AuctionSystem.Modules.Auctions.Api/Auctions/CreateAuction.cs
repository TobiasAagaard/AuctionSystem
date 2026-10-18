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

            await auctionRepository.AddAuctionAsync(@auction, cancellationToken);

            return @auction is not null ? Results.Created($"/auctions/{@auction.Id}", new
            {
                @auction.Id,
            }) : Results.BadRequest();

        })
        .WithTags(Tags.Auctions);
    }
}