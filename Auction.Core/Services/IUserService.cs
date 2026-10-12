using Auction_Core.Models;

namespace Auction_Core.Services;

public interface IUserService
{
    Task DepositAsync(User user, decimal amount);
}
