
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Domain.Auctions;

namespace Auctionly.Modules.Auctions.Application.Auctions.PublishAuction;

internal sealed class PublishAuctionCommandHandler(IAuctionRepository auctionRepository) : ICommandHandler<PublishAuctionCommand, Result<int>>
{
    public Task<Result<int>> Handle(PublishAuctionCommand request, CancellationToken ct) =>
        auctionRepository.PublishAuctionAsync(request.AuctionId, ct);
}