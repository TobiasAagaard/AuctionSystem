using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Auctionly.Modules.Auctions.Api.Auctions;

public static partial class CreateAuction
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auctions", async (CreateAuctionRequest request, IAuctionRepository auctionRepository, CancellationToken cancellationToken) =>
        {

            Vehicle vehicle = ToVehicle(request.Vehicle);

            var @auction = new Auction(
                request.Title,
                request.Description,
                request.Price,
                request.StartTime,
                request.EndTime,
                vehicle
            );

            int auctionId = await auctionRepository.AddAuctionAsync(@auction, cancellationToken);

            return auctionId != 0 ? Results.Created($"/auctions/{auctionId}", new
            {
                Id = auctionId
            }) : Results.BadRequest();

        })
        .WithTags(Tags.Auctions);
    }

    private static Vehicle ToVehicle(VehicleRequest request)
    {
        VehicleDetails details = request switch
        {
            PersonalCarRequest p => new PersonalCar(p.SeatCount, p.Isofix),
            BusinessCarRequest b => new BusinessCar(b.CargoVolume, b.MaxPayload),
            _ => throw new ArgumentException("Unknown vehicle type.", nameof(request))
        };
        return new Vehicle(
            request.Make,
            request.Model,
            request.Year,
            request.Mileage,
            request.BasePrice,
            request.EngineSize,
            request.FuelType,
            details
        );
    }
}