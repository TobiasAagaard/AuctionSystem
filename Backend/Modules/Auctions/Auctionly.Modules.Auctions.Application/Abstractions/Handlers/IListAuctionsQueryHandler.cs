using Auctionly.Modules.Auctions.Application.Auctions.ListAuctions;
namespace Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

public interface IListAuctionsQueryHandler
{
     Task<List<ListAuctionsResponse>> Handle(ListAuctionsQuery request, CancellationToken cancellationToken);
}