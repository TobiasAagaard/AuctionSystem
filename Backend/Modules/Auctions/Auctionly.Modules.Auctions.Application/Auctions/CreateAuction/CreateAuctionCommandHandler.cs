using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Domain.Auctions;
using Auctionly.Modules.Auctions.Domain.Vehicles;
using Auctionly.Modules.Auctions.Domain.ValueObjects;

namespace Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;

internal sealed class CreateAuctionCommandHandler(IAuctionRepository auctionRepository) : ICommandHandler<CreateAuctionCommand, int>
{

    public async Task<int> Handle(CreateAuctionCommand request, CancellationToken ct)
    {
        CreateAuctionCommand.VehicleData v = request.Vehicle;

        var vehicle = new Vehicle(
            v.Make,
            v.Model,
            v.Year,
            v.Mileage,
            new Money(v.PurchasePrice, v.Currency),
            v.EngineSize,
            v.FuelType,
            v.Specifications);

        var auction = new Auction(
            request.Title,
            request.Description,
            new Money(request.Price, request.Currency),
            request.StartTime,
            request.EndTime,
            vehicle);

        return await auctionRepository.AddAuctionAsync(auction, ct);
    }
}
