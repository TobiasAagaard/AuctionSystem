using Auction_Core.Utilities;

namespace Auction.Test.Utilities;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_ReturnsSaltAndHashSeparatedByDelimiter()
    {
        string hash = PasswordHasher.Hash("Password1");

        string[] parts = hash.Split(':');
        Assert.Equal(2, parts.Length);
        Assert.NotEmpty(parts[0]);
        Assert.NotEmpty(parts[1]);
    }

    [Fact]
    public void Hash_ProducesDifferentHashesForSamePassword()
    {
        string first = PasswordHasher.Hash("Password1");
        string second = PasswordHasher.Hash("Password1");

        Assert.NotEqual(first, second);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Hash_ThrowsForNullOrEmptyPassword(string? password)
    {
        Assert.Throws<ArgumentException>(() => PasswordHasher.Hash(password!));
    }

    [Fact]
    public void Verify_ReturnsTrueForCorrectPassword()
    {
        string hash = PasswordHasher.Hash("Password1");

        Assert.True(PasswordHasher.Verify("Password1", hash));
    }

    [Fact]
    public void Verify_ReturnsFalseForIncorrectPassword()
    {
        string hash = PasswordHasher.Hash("Password1");

        Assert.False(PasswordHasher.Verify("WrongPassword", hash));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Verify_ReturnsFalseForNullOrEmptyStoredHash(string? storedHash)
    {
        Assert.False(PasswordHasher.Verify("Password1", storedHash!));
    }

    [Theory]
    [InlineData("no-delimiter")]
    [InlineData("too:many:parts")]
    public void Verify_ReturnsFalseForMalformedStoredHash(string storedHash)
    {
        Assert.False(PasswordHasher.Verify("Password1", storedHash));
    }

    // Precomputed with the current parameters (salt = bytes 0..15, PBKDF2-SHA256,
    // 100_000 iterations, 32-byte key) for password "Password1".
    // If these fail, the hashing parameters changed and every stored password would break.
    private const string KnownPassword = "Password1";
    private const string KnownHash =
        "AAECAwQFBgcICQoLDA0ODw==:0vw0HWGmUxbw76tsP7KpA04QZgD0nNeOpkOrD0gSYYI=";

    [Fact]
    public void Verify_StillAcceptsHashProducedByCurrentParameters()
    {
        Assert.True(PasswordHasher.Verify(KnownPassword, KnownHash));
    }

    [Fact]
    public void Verify_RejectsWrongPasswordAgainstKnownHash()
    {
        Assert.False(PasswordHasher.Verify("WrongPassword1", KnownHash));
    }
}
