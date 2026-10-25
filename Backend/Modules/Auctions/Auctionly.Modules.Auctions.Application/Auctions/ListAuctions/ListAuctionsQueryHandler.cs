using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using System.Data.Common;
using Dapper;
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

namespace Auctionly.Modules.Auctions.Application.Auctions.ListAuctions;

internal sealed class ListAuctionsQueryHandler(IDbConnectionFactory dbConnectionFactory) : IQueryHandler<ListAuctionsQuery, List<ListAuctionsResponse>>
{
    public async Task<List<ListAuctionsResponse>> Handle(ListAuctionsQuery request, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(ct);

        const string query =
        $"""
            SELECT
                id AS {nameof(ListAuctionsResponse.Id)},
                title AS {nameof(ListAuctionsResponse.Title)},
                price AS {nameof(ListAuctionsResponse.Price)},
                currency AS {nameof(ListAuctionsResponse.Currency)}
            FROM auctions.auctions
        """;

        IEnumerable<ListAuctionsResponse> auctions = await connection.QueryAsync<ListAuctionsResponse>(query);

        return auctions.ToList();
    }

}