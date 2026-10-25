using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using System.Data.Common;
using Dapper;
using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;

namespace Auctionly.Modules.Auctions.Application.Auctions.ListAuctions;

public sealed class ListAuctionsQueryHandler(IDbConnectionFactory dbConnectionFactory) : IListAuctionsQueryHandler
{
    public async Task<List<ListAuctionsResponse>> Handle(ListAuctionsQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string query =
        $"""
            SELECT
                id AS {nameof(ListAuctionsResponse.Id)},
                title AS {nameof(ListAuctionsResponse.Title)},
                price AS {nameof(ListAuctionsResponse.Price)}
            FROM auctions.auctions
        """;

        IEnumerable<ListAuctionsResponse> auctions = await connection.QueryAsync<ListAuctionsResponse>(query);

        return auctions.ToList();
    }

}