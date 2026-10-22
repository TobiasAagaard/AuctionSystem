using Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;
using Auctionly.Modules.Auctions.Domain.Vehicles.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using static Auctionly.Modules.Auctions.Presentation.Auctions.Requests.CreateAuctionRequest;

namespace Auctionly.Modules.Auctions.Presentation.Auctions;


internal static class CreateAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auctions", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new CreateAuctionCommand(
                request.Title,
                request.Description,
                request.StartingPrice,
                request.StartTime,
                request.EndTime,
                new CreateAuctionCommand.VehicleData(
                    request.Vehicle.Make,
                    request.Vehicle.Model,
                    request.Vehicle.Year,
                    request.Vehicle.Mileage,
                    request.Vehicle.BasePrice,
                    request.Vehicle.EngineSize,
                    request.Vehicle.FuelType,
                    ToSpecifications(request.Vehicle)
                )
            );

            int auctionId = await sender.Send(command, cancellationToken);

            return Results.Created($"/auctions/{auctionId}", new { Id = auctionId });

        })
        .WithTags(Tags.Auctions);
    }

    private static VehicleSpecifications ToSpecifications(VehicleRequest vehicle) =>
        vehicle switch
        {
            PersonalCarRequest p => new PersonalCarSpecifications(p.SeatCount, p.Isofix),
            BusinessCarRequest b => new BusinessCarSpecifications(b.CargoVolume, b.MaxPayload),
            _ => throw new BadHttpRequestException($"Unknown vehicle type '{vehicle.GetType().Name}'.")
        };
}
