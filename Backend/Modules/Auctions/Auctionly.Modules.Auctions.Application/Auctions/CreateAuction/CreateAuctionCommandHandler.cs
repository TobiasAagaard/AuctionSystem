using Auctionly.Modules.Auctions.Application.Abstractions.Clock;
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Domain.Auctions;
using Auctionly.Modules.Auctions.Domain.Vehicles;
using Auctionly.Modules.Auctions.Domain.ValueObjects;

namespace Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;

internal sealed class CreateAuctionCommandHandler(
    IAuctionRepository auctionRepository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<CreateAuctionCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAuctionCommand request, CancellationToken ct)
    {
        DateTime utcNow = dateTimeProvider.UtcNow;
        CreateAuctionCommand.VehicleData v = request.Vehicle;

        Result<Vehicle> vehicleResult = Vehicle.Create(
            v.Make,
            v.Model,
            v.Year,
            v.Mileage,
            new Money(v.PurchasePrice, v.Currency),
            v.EngineSize,
            v.FuelType,
            v.Specifications,
            utcNow);

        if (vehicleResult.IsFailure)
        {
            return Result.Failure<int>(vehicleResult.Error);
        }

        Result<Auction> auctionResult = Auction.Create(
            request.Title,
            request.Description,
            new Money(request.Price, request.Currency),
            request.StartTime,
            request.EndTime,
            vehicleResult.Value,
            utcNow);

        if (auctionResult.IsFailure)
        {
            return Result.Failure<int>(auctionResult.Error);
        }

        return await auctionRepository.AddAuctionAsync(auctionResult.Value, ct);
    }
}
