
namespace Auctionly.Modules.Auctions.Domain.Vehicles.ValueObjects;

public record PersonalCarSpecifications(
    int SeatCount,
    bool Isofix


) : VehicleSpecifications;
