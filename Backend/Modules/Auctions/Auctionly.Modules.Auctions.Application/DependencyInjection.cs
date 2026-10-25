using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;
using Auctionly.Modules.Auctions.Application.Auctions.GetAuction;
using Auctionly.Modules.Auctions.Application.Auctions.ListAuctions;
using Microsoft.Extensions.DependencyInjection;

namespace Auctionly.Modules.Auctions.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateAuctionCommand, int>, CreateAuctionCommandHandler>();
        services.AddScoped<IQueryHandler<GetAuctionQuery, GetAuctionResponse?>, GetAuctionQueryHandler>();
        services.AddScoped<IQueryHandler<ListAuctionsQuery, List<ListAuctionsResponse>>, ListAuctionsQueryHandler>();
    }
}