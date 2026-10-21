using Auctionly.Modules.Auctions.Application.Abstractions.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Auctionly.Modules.Auctions.Application;

public static class DependencyInjection
{
    // Registers everything the Application layer owns: handlers, behaviors and validators.
    public static IServiceCollection AddAuctionsApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }
}
