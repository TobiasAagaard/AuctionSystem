using Auctionly.Modules.Auctions.Domain.Abstractions;

namespace Auctionly.Modules.Auctions.Domain.Vehicles;

public static class VehicleErrors
{
    public static readonly Error ElectricWithEngineSize = Error.Problem(
        "Vehicle.ElectricWithEngineSize",
        "Electric vehicles must have an engine size of 0");

    public static readonly Error InvalidEngineSize = Error.Problem(
        "Vehicle.InvalidEngineSize",
        "Engine size must be greater than 0 for non-electric vehicles");

    public static readonly Error InvalidYear = Error.Problem(
        "Vehicle.InvalidYear",
        "Year must be 1886 or later");
}
