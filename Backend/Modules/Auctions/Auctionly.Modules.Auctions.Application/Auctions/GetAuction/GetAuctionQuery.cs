using MediatR;

namespace Auctionly.Modules.Auctions.Application.Auctions.GetAuction;

public sealed record GetAuctionQuery(int AuctionId) : IRequest<GetAuctionResponse?>;
