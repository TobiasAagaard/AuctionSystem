using Auction_Core.Models;
using Auction_Core.Services;
using Auction_Core.Utilities;

namespace Auction.Test.Services;

public class AuthServiceTests
{
    [Fact]
    public void Register_ReturnsUserWithHashedPassword()
    {
        var service = new AuthService();

        User user = service.Register("alice", "Password1", "2200");

        Assert.Equal("alice", user.Username);
        Assert.Equal("2200", user.PostalCode);
        Assert.NotEqual("Password1", user.PasswordHash);
        Assert.True(PasswordHasher.Verify("Password1", user.PasswordHash));
    }

    [Fact]
    public void Register_AssignsIncrementingIds()
    {
        var service = new AuthService();

        User first = service.Register("alice", "Password1", "2200");
        User second = service.Register("bob", "Password1", "2200");

        Assert.Equal(first.ID + 1, second.ID);
    }

    [Fact]
    public void Register_ThrowsWhenUsernameAlreadyTaken()
    {
        var service = new AuthService();
        service.Register("alice", "Password1", "2200");

        Assert.Throws<InvalidOperationException>(() => service.Register("ALICE", "Password2", "3000"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    public void Register_ThrowsForInvalidUsername(string username)
    {
        var service = new AuthService();

        Assert.Throws<ArgumentException>(() => service.Register(username, "Password1", "2200"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("short1")]
    [InlineData("nodigitspassword")]
    [InlineData("12345678")]
    public void Register_ThrowsForInvalidPassword(string password)
    {
        var service = new AuthService();

        Assert.Throws<ArgumentException>(() => service.Register("alice", password, "2200"));
    }

    [Fact]
    public void Authenticate_ReturnsUserForValidCredentials()
    {
        var service = new AuthService();
        User registered = service.Register("alice", "Password1", "2200");

        User authenticated = service.Authenticate("alice", "Password1");

        Assert.Equal(registered.ID, authenticated.ID);
    }

    [Fact]
    public void Authenticate_IsCaseInsensitiveForUsername()
    {
        var service = new AuthService();
        service.Register("alice", "Password1", "2200");

        User authenticated = service.Authenticate("ALICE", "Password1");

        Assert.Equal("alice", authenticated.Username);
    }

    [Fact]
    public void Authenticate_ThrowsForUnknownUsername()
    {
        var service = new AuthService();

        Assert.Throws<InvalidOperationException>(() => service.Authenticate("nobody", "Password1"));
    }

    [Fact]
    public void Authenticate_ThrowsForWrongPassword()
    {
        var service = new AuthService();
        service.Register("alice", "Password1", "2200");

        Assert.Throws<InvalidOperationException>(() => service.Authenticate("alice", "WrongPassword1"));
    }

    [Fact]
    public void RegisterThenAuthenticate_RoundTripsSuccessfully()
    {
        var service = new AuthService();
        service.Register("regression-user", "Password1", "2200");

        User user = service.Authenticate("regression-user", "Password1");

        Assert.Equal("regression-user", user.Username);
    }
}
