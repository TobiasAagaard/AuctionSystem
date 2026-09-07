using Auction_Core.Enums;
using Auction_Core.Models;

namespace Auction_Test.Models;

public class SemiTruckTests
{
    private static SemiTruck CreateSemiTruck(bool towBar = false, double engineSize = 12.0) =>
        new SemiTruck(1, "Scania R500", 500_000, "AB12345", 2018, 600_000, towBar, engineSize, 3, 20_000, 4, 9_000, 16);

    [Fact]
    public void Constructor_SetsMaxLoadAndDimensions()
    {
        var truck = CreateSemiTruck();

        Assert.Equal(20_000, truck.MaxLoad);
        Assert.Equal(4, truck.Height);
        Assert.Equal(9_000, truck.Weight);
        Assert.Equal(16, truck.Length);
        Assert.Equal(FuelType.Diesel, truck.FuelType);
    }

    [Theory]
    [InlineData(false, LicenseType.C)]
    [InlineData(true, LicenseType.CE)]
    public void LicenseType_DependsOnTowBar(bool towBar, LicenseType expected)
    {
        var truck = CreateSemiTruck(towBar);

        Assert.Equal(expected, truck.LicenseType);
    }

    [Theory]
    [InlineData(4.2)]
    [InlineData(15.0)]
    public void Constructor_AcceptsEngineSizeAtBoundaries(double engineSize)
    {
        var truck = CreateSemiTruck(engineSize: engineSize);

        Assert.Equal(engineSize, truck.EngineSize);
    }

    [Theory]
    [InlineData(4.19)]
    [InlineData(15.01)]
    public void Constructor_ThrowsForEngineSizeOutsideRange(double engineSize)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateSemiTruck(engineSize: engineSize));
    }

    [Fact]
    public void ToString_IncludesMaxLoad()
    {
        var truck = CreateSemiTruck();

        string result = truck.ToString();

        Assert.Contains("Scania R500", result);
        Assert.Contains("Max Load: 20000 kg", result);
    }
}
