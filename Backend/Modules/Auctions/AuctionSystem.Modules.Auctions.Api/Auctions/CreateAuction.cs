using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public static class CreateAuction
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

    internal sealed class CreateAuctionRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set;}
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}