
using System.Data.Common;
using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Application.Auctions.GetAuction;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Domain.Auctions;
using Dapper;

namespace Auctionly.Modules.Auctions.Application.Auctions.SearchAuctions;

internal sealed class SearchAuctionQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<SearchAuctionQuery, Result<SearchAuctionResponse>>
{
    public async Task<Result<SearchAuctionResponse>> Handle(SearchAuctionQuery request, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(ct);

        var parameters = new SearchAuctionParameters(
            AuctionStatus.Published,
            request.StartTime,
            request.EndTime,
            request.MinPrice,
            request.MaxPrice,
            request.PageSize,
            (request.Page - 1) * request.PageSize
        );

        IReadOnlyList<GetAuctionResponse> auctions = await GetAuctionsAsync(connection, parameters);

        int totalCount = await GetTotalCountAsync(connection, parameters);

        return Result.Success(new SearchAuctionResponse(request.Page, request.PageSize, totalCount, auctions));
        
    }

    private static async Task<IReadOnlyList<GetAuctionResponse>> GetAuctionsAsync(DbConnection connection, SearchAuctionParameters parameters)
    {
        const string query =
        $"""
        SELECT
            id AS {nameof(GetAuctionResponse.Id)},
            title AS {nameof(GetAuctionResponse.Title)},
            description AS {nameof(GetAuctionResponse.Description)},
            start_time AS {nameof(GetAuctionResponse.StartTime)},
            end_time AS {nameof(GetAuctionResponse.EndTime)},
            price AS {nameof(GetAuctionResponse.Price)},
            currency AS {nameof(GetAuctionResponse.Currency)}
        FROM auctions.auctions
        WHERE status = @Status
          AND start_time >= @StartTime
          AND end_time <= @EndTime
          AND price >= @MinPrice
          AND price <= @MaxPrice
        ORDER BY start_time
        LIMIT @Take
        OFFSET @Skip
        """;
        List<GetAuctionResponse> auctions = (await connection.QueryAsync<GetAuctionResponse>(query, parameters)).AsList();
        return auctions;
    }
    
    private static async Task<int> GetTotalCountAsync(DbConnection connection, SearchAuctionParameters parameters)
    {
        const string query =
        $"""
        SELECT COUNT(*)
        FROM auctions.auctions
        WHERE status = @Status
          AND start_time >= @StartTime
          AND end_time <= @EndTime
          AND price >= @MinPrice
          AND price <= @MaxPrice
        """;
        int totalCount = await connection.ExecuteScalarAsync<int>(query, parameters);
        return totalCount;
    }
    
    private sealed record SearchAuctionParameters(
    AuctionStatus Status,
    DateTime StartTime,
    DateTime EndTime,
    decimal MinPrice,
    decimal MaxPrice,
    int Take,
    int Skip
);

}

