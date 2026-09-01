using Microsoft.Extensions.DependencyInjection;
// TODO add the using of Auction.Core.Repositories and Auction.Core.Services here
using Auction_Core.Services;
namespace Auction_Core;

// This class is used to register dependencies for the application layer in the dependency injection container.
public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        
        // TODO register repositories and services here

        services.AddSingleton<AuctionHouse>();

        return services;
    }
}