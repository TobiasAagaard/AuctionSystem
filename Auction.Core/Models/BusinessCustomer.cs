namespace Auction_Core.Models;

public class BusinessCustomer : User
{
    public BusinessCustomer(int id, string username, string passwordHash, string postalCode, decimal credit, string cvr)
        : base(id, username, passwordHash, postalCode)
    {
        Credit = credit;
        CVR = cvr;
    }

    public decimal Credit { get; set; }
    public string CVR { get; }

    public override string ToString()
    {
        return $"{base.ToString()}, Credit: {Credit}, CVR: {CVR}";
    }
}
