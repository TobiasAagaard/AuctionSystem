using AuctionSystem.Modules.Vehicles.Api.Database;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


namespace AuctionSystem.Modules.Vehicles.Api;

public static class VehiclesModule
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        throw new NotImplementedException();
    }

    public static IServiceCollection AddVehiclesModule(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")
            ?? throw new InvalidCastException("Database connection is not configured.");

        
        services.AddSingleton<IVehicleDbConnectionFactory>(new NpgsqlVehicleDbConnectionFactory(connectionString));

        return services;
    }
}