using Auction_Core.Enums;
using Auction_Core.Models;

namespace Auction_Test.Models;

public class AuctionTests
{
    private static Vehicle CreateVehicle() =>
        new PrivatePersonalCar(1, "Golf", 120_000, "AB12345", 2015, 75_000, false, 1.6, 18, FuelType.Petrol, 5, true);

    [Fact]
    public void Constructor_SetsProvidedValuesAndDefaults()
    {
        Vehicle vehicle = CreateVehicle();
        var seller = new TestSeller();

        var auction = new Auction(42, vehicle, seller, 50_000m);

        Assert.Equal(42, auction.Id);
        Assert.Same(vehicle, auction.Vehicle);
        Assert.Same(seller, auction.Seller);
        Assert.Equal(50_000m, auction.MinimumPrice);
        Assert.Equal(0m, auction.HighestBid);
        Assert.Null(auction.HighestBidder);
    }

    [Fact]
    public void Constructor_WithoutNotificationFunction_UsesNonNullNoOp()
    {
        var auction = new Auction(1, CreateVehicle(), new TestSeller(), 10m);

        Assert.NotNull(auction.NotificationFunction);
        auction.NotificationFunction!.Invoke(auction, 10m);
    }

    [Fact]
    public void Constructor_WithNullNotificationFunction_UsesNonNullNoOp()
    {
        var auction = new Auction(1, CreateVehicle(), new TestSeller(), 10m, null);

        Assert.NotNull(auction.NotificationFunction);
    }

    [Fact]
    public void Constructor_KeepsProvidedNotificationFunction()
    {
        decimal received = 0;
        var auction = new Auction(1, CreateVehicle(), new TestSeller(), 10m, (_, bid) => received = bid);

        auction.NotificationFunction!.Invoke(auction, 99m);

        Assert.Equal(99m, received);
    }

    [Fact]
    public void HighestBidAndBidder_AreSettable()
    {
        var auction = new Auction(1, CreateVehicle(), new TestSeller(), 10m);
        var buyer = new TestBuyer();

        auction.HighestBid = 500m;
        auction.HighestBidder = buyer;

        Assert.Equal(500m, auction.HighestBid);
        Assert.Same(buyer, auction.HighestBidder);
    }
}
