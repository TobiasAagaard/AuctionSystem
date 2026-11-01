
using System.Data.Common;
using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Domain.Auctions;
using Auctionly.Modules.Auctions.Domain.ValueObjects.Enums;
using Auctionly.Modules.Auctions.Domain.Vehicles;
using Dapper;

namespace Auctionly.Modules.Auctions.Application.Auctions.SearchAuctions;

internal sealed class SearchAuctionQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<SearchAuctionQuery, Result<SearchAuctionResponse>>
{
    private const int MaxPageSize = 100;

    public async Task<Result<SearchAuctionResponse>> Handle(SearchAuctionQuery request, CancellationToken ct)
    {
        Error? validationError = Validate(request);
        if (validationError is not null)
        {
            return Result.Failure<SearchAuctionResponse>(validationError);
        }

        if (!TryParseEnum(request.FuelType, out FuelType? fuelType))
        {
            return Result.Failure<SearchAuctionResponse>(Error.Problem(
                "Search.InvalidFuelType",
                $"FuelType must be one of: {string.Join(", ", Enum.GetNames<FuelType>())}."));
        }

        if (!TryParseEnum(request.VehicleType, out VehicleType? vehicleType))
        {
            return Result.Failure<SearchAuctionResponse>(Error.Problem(
                "Search.InvalidVehicleType",
                $"VehicleType must be one of: {string.Join(", ", Enum.GetNames<VehicleType>())}."));
        }

        var parameters = new SearchAuctionParameters(
            AuctionStatus.Published.ToString(),
            ToDbTimestamp(request.StartTime),
            ToDbTimestamp(request.EndTime),
            request.MinPrice,
            request.MaxPrice,
            NullIfBlank(request.Make),
            NullIfBlank(request.Model),
            request.MinYear,
            request.MaxYear,
            request.MinMileage,
            request.MaxMileage,
            fuelType?.ToString(),
            vehicleType?.ToString(),
            request.PageSize,
            (request.Page - 1) * request.PageSize
        );

        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(ct);

        IReadOnlyList<SearchAuctionItem> auctions = await GetAuctionsAsync(connection, parameters, ct);

        int totalCount = await GetTotalCountAsync(connection, parameters, ct);

        return Result.Success(new SearchAuctionResponse(request.Page, request.PageSize, totalCount, auctions));
    }

    private static Error? Validate(SearchAuctionQuery request)
    {
        if (request.Page < 1 || request.PageSize < 1 || request.PageSize > MaxPageSize)
        {
            return Error.Problem(
                "Search.InvalidPagination",
                $"Invalid pagination parameters. Page must be greater than 0 and PageSize must be between 1 and {MaxPageSize}.");
        }

        if (request.MinPrice < 0 || request.MaxPrice < 0)
        {
            return Error.Problem("Search.InvalidPrice", "Price filters cannot be negative.");
        }

        if (request.MinPrice > request.MaxPrice)
        {
            return Error.Problem("Search.InvalidPriceRange", "MinPrice cannot be greater than MaxPrice.");
        }

        if (request.StartTime > request.EndTime)
        {
            return Error.Problem("Search.InvalidTimeRange", "StartTime cannot be after EndTime.");
        }

        if (request.MinYear > request.MaxYear)
        {
            return Error.Problem("Search.InvalidYearRange", "MinYear cannot be greater than MaxYear.");
        }

        if (request.MinMileage < 0 || request.MaxMileage < 0)
        {
            return Error.Problem("Search.InvalidMileage", "Mileage filters cannot be negative.");
        }

        if (request.MinMileage > request.MaxMileage)
        {
            return Error.Problem("Search.InvalidMileageRange", "MinMileage cannot be greater than MaxMileage.");
        }

        return null;
    }

    // Case-insensitive, and only accepts names (not numbers like "7"). Blank means "no filter".
    private static bool TryParseEnum<TEnum>(string? value, out TEnum? result) where TEnum : struct, Enum
    {
        result = null;
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        string? name = Enum.GetNames<TEnum>().FirstOrDefault(n => n.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
        if (name is null)
        {
            return false;
        }

        result = Enum.Parse<TEnum>(name);
        return true;
    }

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static DateTime? ToDbTimestamp(DateTime? value) => value switch
    {
        null => null,
        { Kind: DateTimeKind.Unspecified } => value,
        _ => DateTime.SpecifyKind(value.Value.ToUniversalTime(), DateTimeKind.Unspecified)
    };

    private const string FromClause =
    """
        FROM auctions.auctions a
        LEFT JOIN auctions.vehicles v ON v.auction_id = a.id
    """;
    private const string FilterClause =
    """
        WHERE a.status = @Status
            AND (@StartTime::timestamp IS NULL OR a.start_time >= @StartTime)
            AND (@EndTime::timestamp IS NULL OR a.end_time <= @EndTime)
            AND (@MinPrice::decimal IS NULL OR a.price >= @MinPrice)
            AND (@MaxPrice::decimal IS NULL OR a.price <= @MaxPrice)
            AND (@Make::text IS NULL OR v.make ILIKE @Make)
            AND (@Model::text IS NULL OR v.model ILIKE '%' || @Model || '%')
            AND (@MinYear::int IS NULL OR v.year >= @MinYear)
            AND (@MaxYear::int IS NULL OR v.year <= @MaxYear)
            AND (@MinMileage::int IS NULL OR v.mileage >= @MinMileage)
            AND (@MaxMileage::int IS NULL OR v.mileage <= @MaxMileage)
            AND (@FuelType::text IS NULL OR v.fuel_type = @FuelType)
            AND (@VehicleType::text IS NULL OR v.vehicle_type = @VehicleType)
            AND a.is_deleted = FALSE
    """;

    private static async Task<IReadOnlyList<SearchAuctionItem>> GetAuctionsAsync(DbConnection connection, SearchAuctionParameters parameters, CancellationToken ct)
    {
        const string query =
        $"""
        SELECT
            a.id AS {nameof(AuctionRow.Id)},
            a.title AS {nameof(AuctionRow.Title)},
            a.description AS {nameof(AuctionRow.Description)},
            a.start_time AS {nameof(AuctionRow.StartTime)},
            a.end_time AS {nameof(AuctionRow.EndTime)},
            a.price AS {nameof(AuctionRow.Price)},
            a.currency AS {nameof(AuctionRow.Currency)},
            v.make AS {nameof(AuctionRow.Make)},
            v.model AS {nameof(AuctionRow.Model)},
            v.year AS {nameof(AuctionRow.Year)},
            v.mileage AS {nameof(AuctionRow.Mileage)},
            v.engine_size AS {nameof(AuctionRow.EngineSize)},
            v.fuel_type AS {nameof(AuctionRow.FuelType)},
            v.vehicle_type AS {nameof(AuctionRow.VehicleType)}
        {FromClause}
        {FilterClause}
        ORDER BY a.start_time, a.id
        LIMIT @Take
        OFFSET @Skip
        """;
        IEnumerable<AuctionRow> rows = await connection.QueryAsync<AuctionRow>(new CommandDefinition(query, parameters, cancellationToken: ct));
        return rows.Select(r => new SearchAuctionItem(
            r.Id, r.Title, r.Description, r.StartTime, r.EndTime, r.Price, r.Currency,
            new SearchAuctionVehicle(r.Make, r.Model, r.Year, r.Mileage, r.EngineSize, r.FuelType, r.VehicleType)))
            .ToList();
    }


    
    private static async Task<int> GetTotalCountAsync(DbConnection connection, SearchAuctionParameters parameters, CancellationToken ct)
    {
        const string query =
        $"""
        SELECT COUNT(*)
        {FromClause}
        {FilterClause}
        """;
        int totalCount = await connection.ExecuteScalarAsync<int>(new CommandDefinition(query, parameters, cancellationToken: ct));
        return totalCount;
    }
    
        private sealed record AuctionRow
    {
        public int Id { get; init; }
        public string Title { get; init; } = "";
        public string Description { get; init; } = "";
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public decimal Price { get; init; }
        public Currency Currency { get; init; }
        public string Make { get; init; } = "";
        public string Model { get; init; } = "";
        public int Year { get; init; }
        public int Mileage { get; init; }
        public double EngineSize { get; init; }
        public FuelType FuelType { get; init; }
        public VehicleType VehicleType { get; init; }
    }

    private sealed record SearchAuctionParameters(
    string Status,
    DateTime? StartTime,
    DateTime? EndTime,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? Make,
    string? Model,
    int? MinYear,
    int? MaxYear,
    int? MinMileage,
    int? MaxMileage,
    string? FuelType,
    string? VehicleType,
    int Take,
    int Skip
);

}

