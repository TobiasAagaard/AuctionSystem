using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Application.Auctions.GetAuction;
using Auctionly.Modules.Auctions.Application.Auctions.ListAuctions;
using Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;
using Microsoft.Extensions.DependencyInjection;

namespace Auctionly.Modules.Auctions.Application;

public static class DependecyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateAuctionCommandHandler, CreateAuctionCommandHandler>();
        services.AddScoped<IGetAuctionQueryHandler, GetAuctionQueryHandler>();
        services.AddScoped<IListAuctionsQueryHandler, ListAuctionsQueryHandler>();
    }
}