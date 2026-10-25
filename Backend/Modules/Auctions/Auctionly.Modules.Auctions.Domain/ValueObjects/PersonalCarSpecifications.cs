
namespace Auctionly.Modules.Auctions.Domain.ValueObjects;

public record PersonalCarSpecifications(
    int SeatCount,
    bool Isofix


) : VehicleSpecifications;
