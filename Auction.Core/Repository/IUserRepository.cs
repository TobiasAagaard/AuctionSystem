using Auction_Core.Models;

namespace Auction_Core.Repository;

public interface IUserRepository {
    User GetUserById(int id);
    IEnumerable<User> GetAllUsers();
    bool AddUser(string username, string password, string postalCode);
    bool UpdateUser(User user);
    bool DeleteUser(int id);
}