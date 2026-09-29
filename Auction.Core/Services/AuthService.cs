using Auction_Core.Models;
using Auction_Core.Utilities;
using Auction_Core.Repository;
using System.Text.RegularExpressions;

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

    public Task<PrivateCustomer> RegisterPrivateUserAsync(string username, string password, string postalCode, string cpr)
    {
        ValidateUsername(username);
        ValidatePassword(password);
        ValidatePostalCode(postalCode);
        ValidateCpr(cpr);

        string normalizedCpr = cpr.Replace("-", "");
        return _userRepository.AddPrivateCustomerAsync(username, password, postalCode, normalizedCpr);
    }

    public Task<BusinessCustomer> RegisterBusinessUserAsync(string username, string password, string postalCode, string cvr, decimal credit)
    {
        ValidateUsername(username);
        ValidatePassword(password);
        ValidatePostalCode(postalCode);
        ValidateCvr(cvr);

        if (credit < 0)
        {
            throw new ArgumentException("Credit must be a non-negative value");
        }

        return _userRepository.AddBusinessCustomerAsync(username, password, postalCode, cvr, credit);
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
            throw new InvalidOperationException("Invalid username or password");
        }

        return user;
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username is required", nameof(username));
        }
        if (username.Length < MinUsernameLength)
        {
            throw new ArgumentException($"Username must be at least {MinUsernameLength} characters", nameof(username));
        }
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password is required", nameof(password));
        }
        if (password.Length < MinPasswordLength)
        {
            throw new ArgumentException($"Password must be at least {MinPasswordLength} characters", nameof(password));
        }
        if (!password.Any(char.IsDigit) || !password.Any(char.IsLetter))
        {
            throw new ArgumentException("Password must contain both letters and digits", nameof(password));
        }
    }

    private static void ValidatePostalCode(string postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
        {
            throw new ArgumentException("Postal code is required", nameof(postalCode));
        }
        if (postalCode.Length != 4)
        {
            throw new ArgumentException("Postal code must be 4 digits", nameof(postalCode));
        }

        if (!postalCode.All(char.IsDigit))
        {
            throw new ArgumentException("Postal code must contain only digits", nameof(postalCode));
        }
    }

    private static void ValidateCpr(string? cpr)
    {
        if (string.IsNullOrWhiteSpace(cpr))
        {
            throw new ArgumentException("CPR is required", nameof(cpr));
        }

        if (!Regex.IsMatch(cpr, @"^\d{6}-\d{4}$"))
        {
            throw new ArgumentException(
                "CPR must use the format DDMMYY-XXXX",
                nameof(cpr));
        }
    }

    private static void ValidateCvr(string cvr)
    {
        if (string.IsNullOrWhiteSpace(cvr))
        {
            throw new ArgumentException("CVR is required");
        }

        if (cvr.Length != 8 || !cvr.All(char.IsDigit))
        {
            throw new ArgumentException("CVR must be 8 digits");
        }
    }



}
