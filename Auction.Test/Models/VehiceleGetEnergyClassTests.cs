using Auction_Core.Enums;
using Auction_Core.Models;

namespace Auction.Test.Models;

public class VehiceleGetEnergyClassTests
{
    [Theory]
    [InlineData(FuelType.Diesel, 2009, 23, EnergyClass.A)]
    [InlineData(FuelType.Diesel, 20012, 20, EnergyClass.B)]
    [InlineData(FuelType.Petrol, 2009, 17, EnergyClass.C)]
    [InlineData(FuelType.Petrol, 2015, 11, EnergyClass.D)]
    public void GetEnergyClass_ReturnsExpectedEnergyClass(FuelType fuelType, int year, double kmPerLiter, EnergyClass expectedEnergyClass)
    {
       
        var vehicle = new TestVehicle("Test Vehicle", 10000, year, 10000, false, LicenseType.B, 2.0, kmPerLiter, fuelType);

        var energyClass = vehicle.EnergyClass;

        Assert.Equal(expectedEnergyClass, energyClass);
    }

    [Theory]
    [InlineData(FuelType.Electric, 2020, 0, EnergyClass.A)]
    [InlineData(FuelType.Hydrogen, 2020, 0, EnergyClass.A)]
    public void GetEnergyClass_ReturnsAForElectricAndHydrogen(FuelType fuelType, int year, double kmPerLiter, EnergyClass expectedEnergyClass)
    {

        var vehicle = new TestVehicle("Test Vehicle", 10000, year, 10000, false, LicenseType.B, 2.0, kmPerLiter, fuelType);

        var energyClass = vehicle.EnergyClass;

        Assert.Equal(expectedEnergyClass, energyClass);
    }

    private sealed class TestVehicle : Vehicle
    {
        public TestVehicle(string name, double startingPrice, int year, double kilometersDriven, bool isDamaged,
            LicenseType licenseType, double engineSize, double kmPerLiter, FuelType fuelType)
            : base(name, startingPrice, year, kilometersDriven, isDamaged, licenseType, engineSize, kmPerLiter, fuelType)
        {
        }

        public override string ToString()
        {
            return $"{Name} - {Year} - {Kilometers} km - {BasePrice} kr";
        }
    }
}