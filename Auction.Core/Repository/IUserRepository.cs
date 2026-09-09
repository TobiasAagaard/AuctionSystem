using Auction_Core.Models;

namespace Auction_Core.Repository;

public interface IUserRepository {
    Task<User> GetUserByIdAsync(int id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> AddUserAsync(string username, string password, string postalCode);
    Task<PrivateCustomer> AddPrivateCustomerAsync(string username, string password, string postalCode, string cpr);
    Task<BusinessCustomer> AddBusinessCustomerAsync(string username, string password, string postalCode, string cvr, decimal credit);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
}