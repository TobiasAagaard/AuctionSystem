using Auctionly.Modules.Auctions.Domain.Vehicles;
using FluentValidation;

namespace Auctionly.Modules.Auctions.Application.Auctions.SearchAuctions;

internal sealed class SearchAuctionQueryValidator : AbstractValidator<SearchAuctionQuery>
{
    private const int MaxPageSize = 100;

    public SearchAuctionQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, MaxPageSize);

        RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue);
        RuleFor(x => x.MaxPrice).GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue);
        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(x => x.MinPrice)
            .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
            .WithMessage("MinPrice cannot be greater than MaxPrice.");

        RuleFor(x => x.EndTime)
            .GreaterThanOrEqualTo(x => x.StartTime)
            .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
            .WithMessage("StartTime cannot be after EndTime.");

        RuleFor(x => x.Make)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Make))
            .WithMessage("Make cannot be blank.")
            .MaximumLength(100).WithMessage("Make cannot exceed 100 characters.");

        RuleFor(x => x.Model)
            .NotEmpty()
            .When(x => !string.IsNullOrWhiteSpace(x.Model))
            .WithMessage("Model cannot be blank.")
            .MaximumLength(100).WithMessage("Model cannot exceed 100 characters.");
            
        RuleFor(x => x.MaxYear)
            .GreaterThanOrEqualTo(x => x.MinYear)
            .When(x => x.MinYear.HasValue && x.MaxYear.HasValue)
            .WithMessage("MinYear cannot be greater than MaxYear.");

        RuleFor(x => x.MinMileage).GreaterThanOrEqualTo(0).When(x => x.MinMileage.HasValue);
        RuleFor(x => x.MaxMileage).GreaterThanOrEqualTo(0).When(x => x.MaxMileage.HasValue);
        RuleFor(x => x.MaxMileage)
            .GreaterThanOrEqualTo(x => x.MinMileage)
            .When(x => x.MinMileage.HasValue && x.MaxMileage.HasValue)
            .WithMessage("MinMileage cannot be greater than MaxMileage.");

        RuleFor(x => x.FuelType)
            .Must(BeEnumName<FuelType>)
            .WithMessage($"FuelType must be one of: {string.Join(", ", Enum.GetNames<FuelType>())}.");
        RuleFor(x => x.VehicleType)
            .Must(BeEnumName<VehicleType>)
            .WithMessage($"VehicleType must be one of: {string.Join(", ", Enum.GetNames<VehicleType>())}.");
    }

    private static bool BeEnumName<TEnum>(string? value) where TEnum : struct, Enum =>
        string.IsNullOrWhiteSpace(value)
        || Enum.GetNames<TEnum>().Any(n => n.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
}
