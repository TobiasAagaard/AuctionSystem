using Auction_Core.Models;

namespace Auction_Core.Services;

public interface IAuthService {
    Task<User> RegisterAsync(string username, string password, string postalCode);
    Task<User> AuthenticateAsync(string username, string password);
}