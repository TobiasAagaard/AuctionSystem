namespace Auction.Core.Models;

public class User
{

    public User(int id, string username, string password, int postalCode)
    {
        ID = id;
        Username = username;
        Password = password;
        PostalCode = postalCode;
    }

    public int ID { get; }
    public string Username { set; get; }
    public string Password { set; get; }
    public int PostalCode { set; get; }
}
