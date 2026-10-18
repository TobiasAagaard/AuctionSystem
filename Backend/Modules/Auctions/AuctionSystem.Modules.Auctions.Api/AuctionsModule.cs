using AuctionSystem.Modules.Auctions.Api.Database;
using AuctionSystem.Modules.Auctions.Api.Auctions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


namespace AuctionSystem.Modules.Auctions.Api;

public static class AuctionsModule
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        throw new NotImplementedException();
    }

    public static IServiceCollection AddAuctionsModule(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidCastException("Database connection is not configured.");

        
        services.AddSingleton(new NpgsqlAuctionsDbConnectionFactory(connectionString));
        
        services.AddSingleton<IAuctionRepository, AuctionRepository>();

        return services;
    }
}