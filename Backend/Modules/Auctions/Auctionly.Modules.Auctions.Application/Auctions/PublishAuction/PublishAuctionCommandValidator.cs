using Auctionly.Modules.Auctions.Domain.Auctions;
using FluentValidation;

namespace Auctionly.Modules.Auctions.Application.Auctions.PublishAuction;

internal sealed class PublishAuctionCommandValidator : AbstractValidator<PublishAuctionCommand>
{
    public PublishAuctionCommandValidator()
    {
        RuleFor(x => x.Status)
            .Equal(AuctionStatus.Draft)
            .WithMessage("Only draft auctions can be published.");

        RuleFor(x => x.AuctionId)
            .GreaterThan(0)
            .WithMessage("AuctionId must be greater than 0.");
    }
}