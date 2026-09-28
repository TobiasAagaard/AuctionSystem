using Auction_Core.Models;

namespace Auction_Core.Services;

public interface IAuthService {
    Task<PrivateCustomer> RegisterPrivateUserAsync(string username, string password, string postalCode, string cpr);
    Task<BusinessCustomer> RegisterBusinessUserAsync(string username, string password, string postalCode, string cvr, decimal credit);
    Task<User> AuthenticateAsync(string username, string password);
}