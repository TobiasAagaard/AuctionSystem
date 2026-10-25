
namespace Auctionly.Modules.Auctions.Domain.ValueObjects;

public record BusinessCarSpecifications(
    double CargoVolume,
    double MaxPayload
) : VehicleSpecifications;
