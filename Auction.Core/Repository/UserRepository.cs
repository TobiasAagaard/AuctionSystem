using Auction_Core.Models;
using Auction_Core.Utilities;

namespace Auction_Core.Repository;

public class UserRepository : IUserRepository 
{
    private IEnumerable<User> _users = new List<User>();
    private int _nextId = 1;

    public User GetUserById(int id) {

        var user = _users.FirstOrDefault(u => u.ID == id);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {id} not found.");
        }
        return user;
    }

    public IEnumerable<User> GetAllUsers() {

        if (_users == null)
        {
            return new List<User>();
        }

        return _users;
    }

    public bool AddUser(string username, string password, string postalCode) {
        var newUser = new User(
            _nextId++,
            username,
            PasswordHasher.Hash(password),
            postalCode
        );
        _users = _users.Append(newUser).ToList();
        return true;
    }

    public bool UpdateUser(User user) {
        throw new NotImplementedException();
    }

    public bool DeleteUser(int id) {
        throw new NotImplementedException();
    }
}