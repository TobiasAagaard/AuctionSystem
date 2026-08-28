using Microsoft.Extensions.DependencyInjection;
// using Auction.Core.Repositories; // Uncomment this line if you have repositories to register

namespace Auction.Core;

// This class is used to register dependencies for the application layer in the dependency injection container.
public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}