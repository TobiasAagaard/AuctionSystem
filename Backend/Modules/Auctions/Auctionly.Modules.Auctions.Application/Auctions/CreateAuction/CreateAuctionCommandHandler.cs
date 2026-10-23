using Auctionly.Modules.Auctions.Application.Abstractions.Messaging;
using Auctionly.Modules.Auctions.Domain.Auctions;
using Auctionly.Modules.Auctions.Domain.Vehicles;

namespace Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;

internal sealed class CreateAuctionCommandHandler(IAuctionRepository auctionRepository) : ICommandHandler<CreateAuctionCommand, int>
{
    public async Task<int> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
    {
        CreateAuctionCommand.VehicleData v = request.Vehicle;

        var vehicle = new Vehicle(
            v.Make,
            v.Model,
            v.Year,
            v.Mileage,
            v.PurchasePrice,
            v.EngineSize,
            v.FuelType,
            v.Specifications);

        var auction = new Auction(
            request.Title,
            request.Description,
            request.Price,
            request.StartTime,
            request.EndTime,
            vehicle);

        return await auctionRepository.AddAuctionAsync(auction, cancellationToken);
    }
}
