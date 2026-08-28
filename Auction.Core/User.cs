namespace Auction.Core.Models;

public class User
{

    public User(uint id, string username, string password, uint postalCode)
    {
        ID = id;
        Username = username;
        Password = password;
        PostalCode = postalCode;
    }

    public uint ID { get; }
    public string Username { set; get; }
    public string Password { set; get; }
    public uint PostalCode { set; get; }
}
