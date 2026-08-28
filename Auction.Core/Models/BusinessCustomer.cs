namespace Auction.Core.Models;

public class BusinessCustomer : User
{
    public BusinessCustomer(int id, string username, string passwordHash, int postalCode, decimal credit, string cvr)
        : base(id, username, passwordHash, postalCode)
    {
        Credit = credit;
        CVR = cvr;
    }

    public decimal Credit { get; set;}
    public string CVR { get; set; }
}
