using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;
using Auctionly.Modules.Auctions.Application.Auctions.GetAuction;
using Auctionly.Modules.Auctions.Application.Auctions.ListAuctions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Auctionly.Modules.Auctions.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {

        // Register validators from the assembly
        services.AddValidatorsFromAssembly(Application.AssemblyReference.Assembly, includeInternalTypes: true);

        // Register Command Handlers
        services.AddCommandHandler<CreateAuctionCommand, int, CreateAuctionCommandHandler>();

        // Register Query Handlers
        services.AddScoped<IQueryHandler<GetAuctionQuery, GetAuctionResponse?>, GetAuctionQueryHandler>();
        services.AddScoped<IQueryHandler<ListAuctionsQuery, List<ListAuctionsResponse>>, ListAuctionsQueryHandler>();
    }


    // Helper method to register command handlers with validation
    private static void AddCommandHandler<TCommand, TResult, THandler>(this IServiceCollection services) where THandler : class, ICommandHandler<TCommand, TResult>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand, TResult>>(serviceProvider =>
            new ValidationCommandHandler<TCommand, TResult>(
                serviceProvider.GetRequiredService<THandler>(),
                serviceProvider.GetServices<IValidator<TCommand>>()));
    }
}

