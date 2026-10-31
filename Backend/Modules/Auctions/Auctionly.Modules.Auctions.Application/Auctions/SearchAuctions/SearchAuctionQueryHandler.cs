
using System.Data.Common;
using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Domain.Auctions;
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

        var parameters = new SearchAuctionParameters(
            AuctionStatus.Published.ToString(),
            ToDbTimestamp(request.StartTime),
            ToDbTimestamp(request.EndTime),
            request.MinPrice,
            request.MaxPrice,
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

        return null;
    }

    private static DateTime? ToDbTimestamp(DateTime? value) => value switch
    {
        null => null,
        { Kind: DateTimeKind.Unspecified } => value,
        _ => DateTime.SpecifyKind(value.Value.ToUniversalTime(), DateTimeKind.Unspecified)
    };

    private const string FromClause = "FROM auctions.auctions";
    private const string FilterClause =
    """
        WHERE status = @Status
            AND (@StartTime::timestamp IS NULL OR start_time >= @StartTime)
            AND (@EndTime::timestamp IS NULL OR end_time <= @EndTime)
            AND (@MinPrice::decimal IS NULL OR price >= @MinPrice)
            AND (@MaxPrice::decimal IS NULL OR price <= @MaxPrice)
            AND is_deleted = FALSE
    """;

    private static async Task<IReadOnlyList<SearchAuctionItem>> GetAuctionsAsync(DbConnection connection, SearchAuctionParameters parameters, CancellationToken ct)
    {
        const string query =
        $"""
        SELECT
            id AS {nameof(SearchAuctionItem.Id)},
            title AS {nameof(SearchAuctionItem.Title)},
            description AS {nameof(SearchAuctionItem.Description)},
            start_time AS {nameof(SearchAuctionItem.StartTime)},
            end_time AS {nameof(SearchAuctionItem.EndTime)},
            price AS {nameof(SearchAuctionItem.Price)},
            currency AS {nameof(SearchAuctionItem.Currency)}
        {FromClause}
        {FilterClause}
        ORDER BY start_time
        LIMIT @Take
        OFFSET @Skip
        """;
        List<SearchAuctionItem> auctions = (await connection.QueryAsync<SearchAuctionItem>(new CommandDefinition(query, parameters, cancellationToken: ct))).AsList();
        return auctions;
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
    
    private sealed record SearchAuctionParameters(
    string Status,
    DateTime? StartTime,
    DateTime? EndTime,
    decimal? MinPrice,
    decimal? MaxPrice,
    int Take,
    int Skip
);

}

