using Auctionly.Modules.Auctions.Application;
using Auctionly.Modules.Auctions.Domain.Auctions;
using Auctionly.Modules.Auctions.Infrastructure.Auctions;
using Auctionly.Modules.Auctions.Infrastructure.Database;
using Auctionly.Modules.Auctions.Presentation.Auctions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auctionly.Modules.Auctions.Infrastructure;

public static class AuctionsModule
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        AuctionEndpoints.MapEndpoints(app);
    }

    public static IServiceCollection AddAuctionsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuctionsApplication();

        services.AddInfrastructure(configuration);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Database connection is not configured.");

        services.AddSingleton(new NpgsqlAuctionsDbConnectionFactory(connectionString));

        services.AddScoped<IAuctionRepository, AuctionRepository>();
    }
}
