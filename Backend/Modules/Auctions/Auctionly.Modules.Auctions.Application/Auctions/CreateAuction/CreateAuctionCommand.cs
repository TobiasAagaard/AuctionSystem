using Auctionly.Modules.Auctions.Domain.Vehicles;
using Auctionly.Modules.Auctions.Domain.ValueObjects;
using Auctionly.Modules.Auctions.Domain.ValueObjects.Enums;

namespace Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;

public sealed record CreateAuctionCommand(
    string Title,
    string Description,
    decimal Price,
    Currency Currency,
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
        Currency Currency,
        double EngineSize,
        FuelType FuelType,
        VehicleSpecifications Specifications);
}
