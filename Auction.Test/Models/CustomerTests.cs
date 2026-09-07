using Auction_Core.Models;

namespace Auction_Test.Models;

public class CustomerTests
{
    [Fact]
    public void PrivateCustomer_InheritsUserPropertiesAndExposesCpr()
    {
        var customer = new PrivateCustomer(3, "alice", "hash", "2200", "0101901234");

        Assert.Equal(3, customer.ID);
        Assert.Equal("alice", customer.Username);
        Assert.Equal("0101901234", customer.CPR);
        Assert.IsAssignableFrom<User>(customer);
    }

    [Fact]
    public void PrivateCustomer_ToString_AppendsCprToUserDescription()
    {
        var customer = new PrivateCustomer(3, "alice", "hash", "2200", "0101901234");

        string result = customer.ToString();

        Assert.Contains("ID: 3", result);
        Assert.Contains("Username: alice", result);
        Assert.Contains("CPR: 0101901234", result);
    }

    [Fact]
    public void BusinessCustomer_ExposesCreditAndCvr()
    {
        var customer = new BusinessCustomer(4, "acme", "hash", "5000", 25_000m, "12345678");

        Assert.Equal(25_000m, customer.Credit);
        Assert.Equal("12345678", customer.CVR);
        Assert.IsAssignableFrom<User>(customer);
    }

    [Fact]
    public void BusinessCustomer_ToString_AppendsCreditAndCvr()
    {
        var customer = new BusinessCustomer(4, "acme", "hash", "5000", 25_000m, "12345678");

        string result = customer.ToString();

        Assert.Contains("Username: acme", result);
        Assert.Contains($"Credit: {25_000m}", result);
        Assert.Contains("CVR: 12345678", result);
    }

    [Fact]
    public void Customers_CanActAsBuyerAndSeller()
    {
        var customer = new PrivateCustomer(5, "alice", "hash", "2200", "0101901234") { Balance = 1_000m };

        IBuyer buyer = customer;
        ISeller seller = customer;

        Assert.Equal(1_000m, buyer.Balance);
        Assert.Equal(1_000m, seller.Balance);
    }
}
