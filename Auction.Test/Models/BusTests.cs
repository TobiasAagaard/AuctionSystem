using Auction_Core.Enums;
using Auction_Core.Models;

namespace Auction_Test.Models;

public class BusTests
{
    private static Bus CreateBus(bool towBar = false, double engineSize = 8.0) =>
        new Bus(1, "Volvo 9700", 250_000, "AB12345", 2015, 400_000, towBar, engineSize, 4, 12_000, 3.5, 13, 50, 0, true);

    [Fact]
    public void Constructor_SetsBusSpecificProperties()
    {
        var bus = CreateBus();

        Assert.Equal(50, bus.Seats);
        Assert.Equal(0, bus.SleepingPlaces);
        Assert.True(bus.HasToilet);
        Assert.Equal(FuelType.Diesel, bus.FuelType);
    }

    [Fact]
    public void Constructor_MapsHeavyVehicleDimensions()
    {
        var bus = CreateBus();

        Assert.Equal(12_000, bus.Weight);
        Assert.Equal(3.5, bus.Height);
        Assert.Equal(13, bus.Length);
    }

    [Theory]
    [InlineData(false, LicenseType.D)]
    [InlineData(true, LicenseType.DE)]
    public void LicenseType_DependsOnTowBar(bool towBar, LicenseType expected)
    {
        var bus = CreateBus(towBar);

        Assert.Equal(expected, bus.LicenseType);
    }

    [Theory]
    [InlineData(4.2)]
    [InlineData(15.0)]
    public void Constructor_AcceptsEngineSizeAtBoundaries(double engineSize)
    {
        var bus = CreateBus(engineSize: engineSize);

        Assert.Equal(engineSize, bus.EngineSize);
    }

    [Theory]
    [InlineData(4.1)]
    [InlineData(15.1)]
    [InlineData(0)]
    public void Constructor_ThrowsForEngineSizeOutsideRange(double engineSize)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateBus(engineSize: engineSize));
    }

    [Fact]
    public void ToString_IncludesBusDetails()
    {
        var bus = CreateBus();

        string result = bus.ToString();

        Assert.Contains("Volvo 9700", result);
        Assert.Contains("Seats: 50", result);
        Assert.Contains("SleepingPlaces: 0", result);
        Assert.Contains("HasToilet: True", result);
    }
}
