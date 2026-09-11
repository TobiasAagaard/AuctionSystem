using Auction_Core.Models;
using Auction_Core.Repository;
using Auction_Core.Services;
using Auction_Core.Utilities;
using Npgsql;

namespace Auction_Test.Services;


public class AuthServiceTests : IAsyncLifetime
{
    private readonly Database _database = new();
    private readonly AuthService _service = new(new UserRepository(new Database()));
    private readonly List<string> _createdUsernames = [];

    // xUnit creates one instance per test case, so this keeps usernames unique across tests and runs.
    private readonly string _suffix = Guid.NewGuid().ToString("N")[..8];

    private string? _skipReason;

    public async ValueTask InitializeAsync()
    {
        try
        {
            await using NpgsqlConnection connection = await _database.GetConnection();
        }
        catch (Exception exception) when (exception is NpgsqlException or System.Net.Sockets.SocketException)
        {
            _skipReason = $"No test database reachable ({exception.Message}). " +
                          "Start it with 'docker compose up -d database' or set AUCTION_TEST_DB.";
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_skipReason is not null || _createdUsernames.Count == 0)
        {
            return;
        }

        await using NpgsqlConnection connection = await _database.GetConnection();
        using NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM users WHERE username = ANY(@usernames)";
        cmd.Parameters.AddWithValue("usernames", _createdUsernames.ToArray());
        await cmd.ExecuteNonQueryAsync();
    }

    private string Unique(string name)
    {
        string username = $"{name}-{_suffix}";
        _createdUsernames.Add(username);
        return username;
    }

    [Fact]
    public async Task Register_ReturnsUserWithHashedPassword()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);
        string username = Unique("alice");

        User user = await _service.RegisterAsync(username, "Password1", "2200");

        Assert.Equal(username, user.Username);
        Assert.Equal("2200", user.PostalCode);
        Assert.NotEqual("Password1", user.PasswordHash);
        Assert.True(PasswordHasher.Verify("Password1", user.PasswordHash));
    }

    [Fact]
    public async Task Register_AssignsIncrementingIds()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);

        User first = await _service.RegisterAsync(Unique("alice"), "Password1", "2200");
        User second = await _service.RegisterAsync(Unique("bob"), "Password1", "2200");

        Assert.Equal(first.ID + 1, second.ID);
    }

    [Fact]
    public async Task Register_ThrowsWhenUsernameAlreadyTaken()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);
        string username = Unique("alice");
        await _service.RegisterAsync(username, "Password1", "2200");

        await Assert.ThrowsAsync<UsernameAlreadyExistsException>(async () => await _service.RegisterAsync(username.ToUpperInvariant(), "Password2", "3000"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    public async Task Register_ThrowsForInvalidUsername(string username)
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);

        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.RegisterAsync(username, "Password1", "2200"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("short1")]
    [InlineData("nodigitspassword")]
    [InlineData("12345678")]
    public async Task Register_ThrowsForInvalidPassword(string password)
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);

        await Assert.ThrowsAsync<ArgumentException>(async () => await _service.RegisterAsync(Unique("alice"), password, "2200"));
    }

    [Fact]
    public async Task Authenticate_ReturnsUserForValidCredentials()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);
        string username = Unique("alice");
        User registered = await _service.RegisterAsync(username, "Password1", "2200");

        User authenticated = await _service.AuthenticateAsync(username, "Password1");

        Assert.Equal(registered.ID, authenticated.ID);
    }

    [Fact]
    public async Task Authenticate_IsCaseInsensitiveForUsername()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);
        string username = Unique("alice");
        await _service.RegisterAsync(username, "Password1", "2200");

        User authenticated = await _service.AuthenticateAsync(username.ToUpperInvariant(), "Password1");

        Assert.Equal(username, authenticated.Username);
    }

    [Fact]
    public async Task Authenticate_ThrowsForUnknownUsername()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.AuthenticateAsync($"nobody-{_suffix}", "Password1"));
    }

    [Fact]
    public async Task Authenticate_ThrowsForWrongPassword()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);
        string username = Unique("alice");
        await _service.RegisterAsync(username, "Password1", "2200");

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.AuthenticateAsync(username, "WrongPassword1"));
    }

    [Fact]
    public async Task RegisterThenAuthenticate_RoundTripsSuccessfully()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);
        string username = Unique("regression-user");
        await _service.RegisterAsync(username, "Password1", "2200");

        User user = await _service.AuthenticateAsync(username, "Password1");

        Assert.Equal(username, user.Username);
    }

    [Fact]
    public async Task Register_OnlyOneSucceedsUnderConcurrency()
    {
        string username = Unique("racer");

        var attempts = Enumerable.Range(0, 8)
            .Select(_ => Task.Run(() => _service.RegisterAsync(username, "Password1", "2200")));

        var results = await Task.WhenAll(attempts.Select(async t =>
        {
            try { await t; return true; } catch (UsernameAlreadyExistsException) { return false; }
        }));

        Assert.Equal(1, results.Count(succeeded => succeeded));
    }
}
