using Auction_Core.Models;
using Auction_Core.Utilities;

namespace Auction_Core.Services;

public class AuthService
{
    private const int MinUsernameLength = 3;
    private const int MinPasswordLength = 8;

    private readonly Dictionary<string, User> _usersByUsername = new(StringComparer.OrdinalIgnoreCase);
    private int _nextId = 1;

    public User Register(string username, string password, string postalCode)
    {
        ValidateUsername(username);
        ValidatePassword(password);

        if (_usersByUsername.ContainsKey(username))
        {
            throw new InvalidOperationException($"Username '{username}' is already taken.");
        }

        string passwordHash = PasswordHasher.Hash(password);
        var user = new User(_nextId++, username, passwordHash, postalCode);
        _usersByUsername[username] = user;

        return user;
    }

    public User Authenticate(string username, string password)
    {
        if (!_usersByUsername.TryGetValue(username, out User? user)
            || !PasswordHasher.Verify(password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid username or password.");
        }

        return user;
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required.", nameof(username));
        }
        if (username.Length < MinUsernameLength)
        {
            throw new ArgumentException($"Username must be at least {MinUsernameLength} characters.", nameof(username));
        }
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password is required.", nameof(password));
        }
        if (password.Length < MinPasswordLength)
        {
            throw new ArgumentException($"Password must be at least {MinPasswordLength} characters.", nameof(password));
        }
        if (!password.Any(char.IsDigit) || !password.Any(char.IsLetter))
        {
            throw new ArgumentException("Password must contain both letters and digits.", nameof(password));
        }
    }
}
