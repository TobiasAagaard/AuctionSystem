namespace Auctionly.Modules.Auctions.Api.Auctions;

public static partial class CreateAuction
{
    internal sealed class CreateAuctionRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set;}
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public VehicleRequest Vehicle { get; set; }
    }
}
