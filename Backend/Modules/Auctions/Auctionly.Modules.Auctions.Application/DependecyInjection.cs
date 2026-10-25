using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;
using Auctionly.Modules.Auctions.Application.Auctions.GetAuction;
using Microsoft.Extensions.DependencyInjection;
namespace Auctionly.Modules.Auctions.Application;

public static class DependecyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateAuctionCommandHandler, CreateAuctionCommandHandler>();
        services.AddScoped<IGetAuctionQueryHandler, GetAuctionQueryHandler>();
    }
}