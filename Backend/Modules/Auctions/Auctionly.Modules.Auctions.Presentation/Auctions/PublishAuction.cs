using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Application.Auctions.PublishAuction;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Presentation.ApiResults;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auctionly.Modules.Auctions.Presentation.Auctions;

internal sealed class PublishAuction
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPut("/Auctions/Publish/{id:int}", async (
            int id,
            ICommandHandler<PublishAuctionCommand, Result<int>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<int> result = await handler.Handle(new PublishAuctionCommand(id), cancellationToken);
            return result.Match(
                auctionId => Results.Ok(new { AuctionId = auctionId }),
                ApiResults.ApiResults.Problem);
        })
        .WithTags(Tags.Auctions)
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);
    }
}