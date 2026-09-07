using Auction_Core.Enums;
using Auction_Core.Models;
using Auction_Core.Services;

namespace Auction_Test.Services;

public class AuctionServiceTests
{
    private static Vehicle CreateVehicle() =>
        new PrivatePersonalCar(1, "Golf", 120_000, "AB12345", 2015, 75_000, false, 1.6, 18, FuelType.Petrol, 5, true);

    private static AuctionService CreateService(out FakeAuctionRepository repository)
    {
        repository = new FakeAuctionRepository();
        return new AuctionService(repository);
    }

    [Fact]
    public void Constructor_ThrowsWhenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new AuctionService(null!));
    }

    [Fact]
    public void SetForSale_AddsAuctionToRepository()
    {
        AuctionService service = CreateService(out FakeAuctionRepository repository);
        var seller = new TestSeller();

        service.SetForSale(CreateVehicle(), seller, 50_000m);

        Assert.Single(repository.GetAllAuctions());
    }

    [Fact]
    public void SetForSale_ThrowsWhenVehicleIsNull()
    {
        AuctionService service = CreateService(out _);

        Assert.Throws<ArgumentNullException>(() => service.SetForSale(null!, new TestSeller(), 10m));
    }

    [Fact]
    public void SetForSale_ThrowsWhenSellerIsNull()
    {
        AuctionService service = CreateService(out _);

        Assert.Throws<ArgumentNullException>(() => service.SetForSale(CreateVehicle(), null!, 10m));
    }

    [Fact]
    public void SetForSale_ThrowsWhenNotificationFunctionIsNull()
    {
        AuctionService service = CreateService(out _);

        Assert.Throws<ArgumentNullException>(() => service.SetForSale(CreateVehicle(), new TestSeller(), 10m, null!));
    }

    [Fact]
    public void SetForSale_ThrowsWhenMinimumPriceIsNegative()
    {
        AuctionService service = CreateService(out _);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.SetForSale(CreateVehicle(), new TestSeller(), -1m));
    }

    [Fact]
    public void ReceiveBid_ThrowsWhenBuyerIsNull()
    {
        AuctionService service = CreateService(out _);

        Assert.Throws<ArgumentNullException>(() => service.ReceiveBid(null!, 1, 10m));
    }

    [Fact]
    public void ReceiveBid_ThrowsWhenBidIsNegative()
    {
        AuctionService service = CreateService(out _);

        Assert.Throws<ArgumentOutOfRangeException>(() => service.ReceiveBid(new TestBuyer(), 1, -1m));
    }

    [Fact]
    public void ReceiveBid_ReturnsFalseForUnknownAuction()
    {
        AuctionService service = CreateService(out _);

        Assert.False(service.ReceiveBid(new TestBuyer { Balance = 100m }, 999, 10m));
    }

    [Fact]
    public void ReceiveBid_RecordsBidAndBidder()
    {
        AuctionService service = CreateService(out FakeAuctionRepository repository);
        service.SetForSale(CreateVehicle(), new TestSeller(), 50_000m);
        var buyer = new TestBuyer { Balance = 100_000m };

        bool result = service.ReceiveBid(buyer, 1, 60_000m);

        Assert.True(result);
        Assert.Equal(60_000m, repository.GetAuctionById(1).HighestBid);
        Assert.Same(buyer, repository.GetAuctionById(1).HighestBidder);
    }

    [Fact]
    public void ReceiveBid_ReturnsFalseWhenBidIsNotHigherThanCurrentBid()
    {
        AuctionService service = CreateService(out FakeAuctionRepository repository);
        service.SetForSale(CreateVehicle(), new TestSeller(), 50_000m);
        var buyer = new TestBuyer { Balance = 100_000m };
        service.ReceiveBid(buyer, 1, 60_000m);

        Assert.False(service.ReceiveBid(new TestBuyer { Balance = 100_000m }, 1, 60_000m));
        Assert.Equal(60_000m, repository.GetAuctionById(1).HighestBid);
    }

    [Fact]
    public void ReceiveBid_ReturnsFalseWhenBuyerCannotAffordBid()
    {
        AuctionService service = CreateService(out FakeAuctionRepository repository);
        service.SetForSale(CreateVehicle(), new TestSeller(), 50_000m);

        bool result = service.ReceiveBid(new TestBuyer { Balance = 100m }, 1, 60_000m);

        Assert.False(result);
        Assert.Equal(0m, repository.GetAuctionById(1).HighestBid);
    }

    [Fact]
    public void ReceiveBid_NotifiesSellerWhenBidReachesMinimumPrice()
    {
        AuctionService service = CreateService(out _);
        var seller = new TestSeller();
        service.SetForSale(CreateVehicle(), seller, 50_000m);

        service.ReceiveBid(new TestBuyer { Balance = 100_000m }, 1, 50_000m);

        (_, decimal bid) = Assert.Single(seller.Notifications);
        Assert.Equal(50_000m, bid);
    }

    [Fact]
    public void ReceiveBid_DoesNotNotifySellerBelowMinimumPrice()
    {
        AuctionService service = CreateService(out _);
        var seller = new TestSeller();
        service.SetForSale(CreateVehicle(), seller, 50_000m);

        service.ReceiveBid(new TestBuyer { Balance = 100_000m }, 1, 49_999m);

        Assert.Empty(seller.Notifications);
    }

    [Fact]
    public void ReceiveBid_UsesCustomNotificationFunction()
    {
        AuctionService service = CreateService(out _);
        decimal notified = 0;
        service.SetForSale(CreateVehicle(), new TestSeller(), 50_000m, (_, bid) => notified = bid);

        service.ReceiveBid(new TestBuyer { Balance = 100_000m }, 1, 55_000m);

        Assert.Equal(55_000m, notified);
    }

    [Fact]
    public void AcceptBid_ThrowsWhenSellerIsNull()
    {
        AuctionService service = CreateService(out _);

        Assert.Throws<ArgumentNullException>(() => service.AcceptBid(null!, 1));
    }

    [Fact]
    public void AcceptBid_ReturnsFalseForUnknownAuction()
    {
        AuctionService service = CreateService(out _);

        Assert.False(service.AcceptBid(new TestSeller(), 999));
    }

    [Fact]
    public void AcceptBid_ReturnsFalseForDifferentSeller()
    {
        AuctionService service = CreateService(out _);
        service.SetForSale(CreateVehicle(), new TestSeller(), 50_000m);
        service.ReceiveBid(new TestBuyer { Balance = 100_000m }, 1, 60_000m);

        Assert.False(service.AcceptBid(new TestSeller(), 1));
    }

    [Fact]
    public void AcceptBid_ReturnsFalseWhenThereIsNoBidder()
    {
        AuctionService service = CreateService(out _);
        var seller = new TestSeller();
        service.SetForSale(CreateVehicle(), seller, 50_000m);

        Assert.False(service.AcceptBid(seller, 1));
    }

    [Fact]
    public void AcceptBid_ReturnsFalseWhenHighestBidIsBelowMinimumPrice()
    {
        AuctionService service = CreateService(out _);
        var seller = new TestSeller();
        service.SetForSale(CreateVehicle(), seller, 50_000m);
        service.ReceiveBid(new TestBuyer { Balance = 100_000m }, 1, 40_000m);

        Assert.False(service.AcceptBid(seller, 1));
    }

    [Fact]
    public void AcceptBid_ReturnsFalseWhenBuyerBalanceDroppedBelowBid()
    {
        AuctionService service = CreateService(out _);
        var seller = new TestSeller();
        service.SetForSale(CreateVehicle(), seller, 50_000m);
        var buyer = new TestBuyer { Balance = 100_000m };
        service.ReceiveBid(buyer, 1, 60_000m);
        buyer.Balance = 100m;

        Assert.False(service.AcceptBid(seller, 1));
    }

    [Fact]
    public void AcceptBid_TransfersMoneyAndMovesAuctionToSold()
    {
        AuctionService service = CreateService(out FakeAuctionRepository repository);
        var seller = new TestSeller { Balance = 1_000m };
        service.SetForSale(CreateVehicle(), seller, 50_000m);
        var buyer = new TestBuyer { Balance = 100_000m };
        service.ReceiveBid(buyer, 1, 60_000m);

        bool result = service.AcceptBid(seller, 1);

        Assert.True(result);
        Assert.Equal(40_000m, buyer.Balance);
        Assert.Equal(61_000m, seller.Balance);
        Assert.Empty(repository.GetAllAuctions());
        Assert.Single(service.SoldAuctions);
        Assert.Equal(1, service.SoldAuctions[0].Id);
    }

    [Fact]
    public void AcceptBid_ReturnsFalseWhenCalledTwice()
    {
        AuctionService service = CreateService(out _);
        var seller = new TestSeller();
        service.SetForSale(CreateVehicle(), seller, 50_000m);
        service.ReceiveBid(new TestBuyer { Balance = 100_000m }, 1, 60_000m);
        service.AcceptBid(seller, 1);

        Assert.False(service.AcceptBid(seller, 1));
        Assert.Single(service.SoldAuctions);
    }
}
