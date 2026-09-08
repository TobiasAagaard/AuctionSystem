using Auction_Core.Enums;
using Auction_Core.Models;

namespace Auction_Test.Models;

public class PersonalCarTests
{
    private static PrivatePersonalCar CreatePrivateCar(
        double engineSize = 1.6,
        int seatCount = 5,
        LicenseType licenseType = LicenseType.B) =>
        new PrivatePersonalCar(1, "Golf", 120_000, "AB12345", 2015, 75_000, false, engineSize, 18, FuelType.Petrol, seatCount, true, licenseType);

    private static BusinessPersonalCar CreateBusinessCar(int seatCount = 2, double cargoCapacity = 500) =>
        new BusinessPersonalCar(2, "Transit", 90_000, "CD67890", 2019, 120_000, 2.0, 15, FuelType.Diesel, seatCount, false, cargoCapacity);

    [Fact]
    public void PrivatePersonalCar_SetsProperties()
    {
        var car = CreatePrivateCar();

        Assert.Equal(5, car.SeatCount);
        Assert.True(car.Isofix);
        Assert.Equal(LicenseType.B, car.LicenseType);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(7)]
    public void PrivatePersonalCar_AcceptsSeatCountAtBoundaries(int seatCount)
    {
        var car = CreatePrivateCar(seatCount: seatCount);

        Assert.Equal(seatCount, car.SeatCount);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(8)]
    [InlineData(0)]
    public void PrivatePersonalCar_ThrowsForSeatCountOutsideRange(int seatCount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreatePrivateCar(seatCount: seatCount));
    }

    [Theory]
    [InlineData(0.7)]
    [InlineData(10.0)]
    public void PersonalCar_AcceptsEngineSizeAtBoundaries(double engineSize)
    {
        var car = CreatePrivateCar(engineSize);

        Assert.Equal(engineSize, car.EngineSize);
    }

    [Theory]
    [InlineData(0.69)]
    [InlineData(10.01)]
    public void PersonalCar_ThrowsForEngineSizeOutsideRange(double engineSize)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreatePrivateCar(engineSize));
    }

    [Theory]
    [InlineData(LicenseType.A)]
    [InlineData(LicenseType.C)]
    [InlineData(LicenseType.D)]
    [InlineData(LicenseType.CE)]
    public void PersonalCar_ThrowsForNonCarLicenseType(LicenseType licenseType)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreatePrivateCar(licenseType: licenseType));
    }

    [Fact]
    public void PersonalCar_AcceptsBeLicenseType()
    {
        var car = CreatePrivateCar(licenseType: LicenseType.BE);

        Assert.Equal(LicenseType.BE, car.LicenseType);
    }

    [Fact]
    public void PrivatePersonalCar_ToString_IncludesSeatCountAndIsofix()
    {
        string result = CreatePrivateCar().ToString();

        Assert.Contains("SeatCount: 5", result);
        Assert.Contains("Isofix: True", result);
    }

    [Fact]
    public void BusinessPersonalCar_AlwaysHasTowBar()
    {
        var car = CreateBusinessCar();

        Assert.True(car.TowBar);
    }

    [Theory]
    [InlineData(750, LicenseType.B)]
    [InlineData(750.1, LicenseType.BE)]
    [InlineData(1_200, LicenseType.BE)]
    public void BusinessPersonalCar_LicenseTypeDependsOnCargoCapacity(double cargoCapacity, LicenseType expected)
    {
        var car = CreateBusinessCar(cargoCapacity: cargoCapacity);

        Assert.Equal(expected, car.LicenseType);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void BusinessPersonalCar_ThrowsWhenSeatCountIsNotTwo(int seatCount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateBusinessCar(seatCount));
    }

    [Fact]
    public void BusinessPersonalCar_ToString_IncludesRollCageAndCargoCapacity()
    {
        string result = CreateBusinessCar().ToString();

        Assert.Contains("RollCage: False", result);
        Assert.Contains("CargoCapacity: 500", result);
    }
}
