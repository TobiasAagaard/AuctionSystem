using Auction_Core.Models;

namespace Auction.Test.Models;

public class UserTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var user = new User(1, "alice", "hash", 2200);

        Assert.Equal(1, user.ID);
        Assert.Equal("alice", user.Username);
        Assert.Equal("hash", user.PasswordHash);
        Assert.Equal(2200, user.PostalCode);
        Assert.Equal(0m, user.Balance);
    }

    [Fact]
    public void ToString_ContainsIdUsernamePostalCodeAndBalance()
    {
        var user = new User(7, "bob", "hash", 5000) { Balance = 123.45m };

        string result = user.ToString();

        Assert.Contains("ID: 7", result);
        Assert.Contains("Username: bob", result);
        Assert.Contains("PostalCode: 5000", result);
        Assert.Contains($"Balance: {123.45m}", result);
    }

    [Fact]
    public void ReceiveNotificationOfBid_ThrowsNotImplemented()
    {
        var user = new User(1, "alice", "hash", 2200);

        Assert.Throws<NotImplementedException>(() => user.ReceiveNotificationOfBid(null!, 100m));
    }
}
