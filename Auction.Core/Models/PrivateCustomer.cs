namespace Auction_Core.Models;

public class PrivateCustomer : User
{
    public PrivateCustomer(int id, string username, string passwordHash, int postalCode, string cpr)
        : base(id, username, passwordHash, postalCode)
    {
        CPR = cpr;
    }

    public string CPR { get; }
}
