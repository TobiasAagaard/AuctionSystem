
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Domain.Auctions;
namespace Auctionly.Modules.Auctions.Application.Auctions.PublishAuction;

internal sealed class PublishAuctionCommandHandler(IAuctionRepository auctionRepository) : ICommandHandler<PublishAuctionCommand, Result<int>>
{
    public async Task<Result<int>> Handle(PublishAuctionCommand request, CancellationToken ct)
    {
        Result<Auction> auctionResult = Auction.Publish(
            request.Status,
            DateTime.UtcNow
        );

        if (auctionResult.IsFailure)
        {
            return Result.Failure<int>(auctionResult.Error);
        }

        Auction auction = auctionResult.Value;
        int auctionId = await auctionRepository.PublishAuctionAsync(auction, ct);
        return Result.Success(auctionId);
    }
}