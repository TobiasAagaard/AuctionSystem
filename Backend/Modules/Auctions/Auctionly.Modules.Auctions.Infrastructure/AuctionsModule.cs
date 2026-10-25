using System.Reflection.Metadata;
using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using Auctionly.Modules.Auctions.Application;
using Auctionly.Modules.Auctions.Domain.Auctions;
using Auctionly.Modules.Auctions.Infrastructure.Auctions;
using Auctionly.Modules.Auctions.Infrastructure.Database;
using Auctionly.Modules.Auctions.Presentation.Auctions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using FluentValidation;

namespace Auctionly.Modules.Auctions.Infrastructure;

public static class AuctionsModule
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        AuctionEndpoints.MapEndpoints(app);
    }

    public static IServiceCollection AddAuctionsModule(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Database connection is not configured.");

        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.AddScoped<IAuctionRepository, AuctionRepository>();
    }

}
