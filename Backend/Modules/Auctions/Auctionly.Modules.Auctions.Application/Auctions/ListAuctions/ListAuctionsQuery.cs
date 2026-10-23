using MediatR;

namespace Auctionly.Modules.Auctions.Application.Auctions.ListAuctions;

public sealed record ListAuctionsQuery() : IRequest<List<ListAuctionsResponse>>;
