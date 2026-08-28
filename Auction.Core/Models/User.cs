namespace Auction.Core.Models;

public class User : ISeller, IBuyer
{

    public User(int id, string username, string passwordHash, int postalCode)
    {
        ID = id;
        Username = username;
        PasswordHash = passwordHash;
        PostalCode = postalCode;
    }

    public int ID { get; }
    public string Username { get; set; }
    public string PasswordHash { get; private set; }
    public int PostalCode { get; set; }
    public decimal Balance { set; get; }

    public void ReceiveNotificationOfBid(Auction auction, decimal bid)
    {
    }
}
