
namespace Auctionly.Modules.Auctions.Domain.Vehicles.ValueObjects;

public record BusinessCarSpecifications(
    double CargoVolume,
    double MaxPayload
) : VehicleSpecifications;
