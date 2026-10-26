
using System.Data.Common;
using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Domain.Auctions;
using Dapper;

namespace Auctionly.Modules.Auctions.Application.Auctions.SearchAuctions;

internal sealed class SearchAuctionQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<SearchAuctionQuery, Result<SearchAuctionResponse>>
{
    public async Task<Result<SearchAuctionResponse>> Handle(SearchAuctionQuery request, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(ct);

        if (request.Page < 1 || request.PageSize < 1 || request.PageSize > 100)
        {
            return Result.Failure<SearchAuctionResponse>(Error.Problem(
                "Search.InvalidPagination",
                "Invalid pagination parameters. Page must be greater than 0 and PageSize must be between 1 and 100."
            ));
        }

        var parameters = new SearchAuctionParameters(
            AuctionStatus.Published.ToString(),
            request.StartTime,
            request.EndTime,
            request.MinPrice,
            request.MaxPrice,
            request.PageSize,
            (request.Page - 1) * request.PageSize
        );

        IReadOnlyList<SearchAuctionItem> auctions = await GetAuctionsAsync(connection, parameters, ct);

        int totalCount = await GetTotalCountAsync(connection, parameters, ct);

        return Result.Success(new SearchAuctionResponse(request.Page, request.PageSize, totalCount, auctions));
        
    }

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
        FROM auctions.auctions
        WHERE status = @Status
          AND (@StartTime IS NULL OR start_time >= @StartTime)
          AND (@EndTime IS NULL OR end_time <= @EndTime)
          AND (@MinPrice::decimal IS NULL OR price >= @MinPrice)
          AND (@MaxPrice::decimal IS NULL OR price <= @MaxPrice)
          AND is_deleted = FALSE
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
        FROM auctions.auctions
        WHERE status = @Status
          AND (@StartTime IS NULL OR start_time >= @StartTime)
          AND (@EndTime IS NULL OR end_time <= @EndTime)
          AND (@MinPrice::decimal IS NULL OR price >= @MinPrice)
          AND (@MaxPrice::decimal IS NULL OR price <= @MaxPrice)
          AND is_deleted = FALSE
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

