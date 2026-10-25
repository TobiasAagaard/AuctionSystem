using Auctionly.Modules.Auctions.Domain.Vehicles;
using Auctionly.Modules.Auctions.Domain.ValueObjects;

namespace Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;

public sealed record CreateAuctionCommand(
    string Title,
    string Description,
    decimal Price,
    string Currency,
    DateTime StartTime,
    DateTime EndTime,
    CreateAuctionCommand.VehicleData Vehicle) 
{
    public sealed record VehicleData(
        string Make,
        string Model,
        int Year,
        int Mileage,
        decimal PurchasePrice,
        string Currency,
        double EngineSize,
        FuelType FuelType,
        VehicleSpecifications Specifications);
}
