using Microsoft.Extensions.DependencyInjection;
// using Auction.Core.Repositories; // Uncomment this line if you have repositories to register

namespace Auction_Core;

// This class is used to register dependencies for the application layer in the dependency injection container.
public static class CoreServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        
        return services;
    }
}