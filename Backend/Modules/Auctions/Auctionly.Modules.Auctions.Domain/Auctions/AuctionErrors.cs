using Auctionly.Modules.Auctions.Domain.Abstractions;

namespace Auctionly.Modules.Auctions.Domain.Auctions;

public static class AuctionErrors
{
    public static Error NotFound(int auctionId) =>
        Error.NotFound("Auction.NotFound", $"The auction with the identifier {auctionId} was not found");

    public static readonly Error StartDateInPast = Error.Problem(
        "Auction.StartDateInPast",
        "The auction start date is in the past");

    public static readonly Error EndDatePrecedesStartDate = Error.Problem(
        "Auction.EndDatePrecedesStartDate",
        "The auction end date precedes the start date");
}
