using Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;

namespace Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

public interface ICreateAuctionCommandHandler
{
    Task<int> Handle(CreateAuctionCommand request, CancellationToken cancellationToken);
}