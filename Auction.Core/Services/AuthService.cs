using Auction_Core.Models;
using Auction_Core.Utilities;
using Auction_Core.Repository;

namespace Auction_Core.Services;

public class AuthService : IAuthService
{
    private const int MinUsernameLength = 3;
    private const int MinPasswordLength = 8;
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<User> RegisterAsync(string username, string password, string postalCode)
    {
        ValidateUsername(username);
        ValidatePassword(password);

        return await _userRepository.AddUserAsync(username, password, postalCode);
    }

    public async Task<User> AuthenticateAsync(string username, string password)
    {

        User user;
        try
        {
            user = await _userRepository.GetUserByUsernameAsync(username);
            
        } catch (InvalidOperationException)
        {
            user = new User(0, username, "", "");
        }
        if (!PasswordHasher.Verify(password, user.PasswordHash))
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
