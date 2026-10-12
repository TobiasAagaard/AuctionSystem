using Auction_Core.Models;
using Auction_Core.Repository;

namespace Auction_Core.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository), "User repository cannot be null.");
    }

    public async Task DepositAsync(User user, decimal amount)
    {
        if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Deposit must be greater than zero.");

        user.Balance = await _userRepository.AddToBalanceAsync(user.ID, amount);
    }
}
