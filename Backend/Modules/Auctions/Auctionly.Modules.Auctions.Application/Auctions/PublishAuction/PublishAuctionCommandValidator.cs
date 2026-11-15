using FluentValidation;

namespace Auctionly.Modules.Auctions.Application.Auctions.PublishAuction;

internal sealed class PublishAuctionCommandValidator : AbstractValidator<PublishAuctionCommand>
{
    public PublishAuctionCommandValidator()
    {
        RuleFor(x => x.AuctionId)
            .GreaterThan(0)
            .WithMessage("AuctionId must be greater than 0.");
    }
}