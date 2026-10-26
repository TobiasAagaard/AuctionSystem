using FluentValidation;
using Auctionly.Modules.Auctions.Domain.Vehicles;

namespace Auctionly.Modules.Auctions.Application.Auctions.CreateAuction;

internal sealed class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
{
    public CreateAuctionCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Currency).IsInEnum();
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndTime).Must((cmd, endTime) => endTime > cmd.StartTime).WithMessage("End time must be after start time.");
        RuleFor(x => x.Vehicle)
            .NotNull()
            .SetValidator(new VehicleDataValidator());
    }
}

internal sealed class VehicleDataValidator : AbstractValidator<CreateAuctionCommand.VehicleData>
{
    public VehicleDataValidator()
    {
        RuleFor(x => x.Make).NotEmpty();
        RuleFor(x => x.Model).NotEmpty();
        RuleFor(x => x.Year).GreaterThan(1885).LessThanOrEqualTo(DateTime.UtcNow.Year);
        RuleFor(x => x.Mileage).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PurchasePrice).GreaterThan(0);
        RuleFor(x => x.Currency).IsInEnum();
        RuleFor(x => x.EngineSize).Equal(0).When(x => x.FuelType == FuelType.Electric);
        RuleFor(x => x.EngineSize).GreaterThan(0).When(x => x.FuelType != FuelType.Electric);
        RuleFor(x => x.FuelType).IsInEnum();
        RuleFor(x => x.Specifications).NotNull();
    }
}
