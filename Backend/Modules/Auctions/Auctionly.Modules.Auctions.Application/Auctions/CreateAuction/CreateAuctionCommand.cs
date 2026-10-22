using Auctionly.Modules.Auctions.Application.Abstractions.Messaging;
using Auctionly.Modules.Auctions.Domain.Vehicles;
using Auctionly.Modules.Auctions.Domain.Vehicles.ValueObjects;

namespace Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;

public sealed record CreateAuctionCommand(
    string Title,
    string Description,
    decimal StartingPrice,
    DateTime StartTime,
    DateTime EndTime,
    CreateAuctionCommand.VehicleData Vehicle) : ICommand<int>
{
    public sealed record VehicleData(
        string Make,
        string Model,
        int Year,
        int Mileage,
        decimal BasePrice,
        double EngineSize,
        FuelType FuelType,
        VehicleSpecifications Specifications);
}
