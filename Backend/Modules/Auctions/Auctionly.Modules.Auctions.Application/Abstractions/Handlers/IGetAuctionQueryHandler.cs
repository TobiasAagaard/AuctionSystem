using Auctionly.Modules.Auctions.Application.Auctions.GetAuction;

namespace Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

public interface IGetAuctionQueryHandler
{
    Task<GetAuctionResponse?> Handle(GetAuctionQuery request, CancellationToken cancellationToken);
}