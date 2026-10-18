using AuctionSystem.Modules.Auctions.Api.Database;
using System.Data;
using Dapper;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public sealed class AuctionRepository(
    NpgsqlAuctionsDbConnectionFactory connectionFactory) : IAuctionRepository
{
    public async Task AddAuctionAsync(Auction auction, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = (
            sql: """
                INSERT INTO auctions.auctions
                    (title, description, price, start_time, end_time)
                VALUES
                    (@Title, @Description, @Price, @StartTime, @EndTime)
                """,
            param: new
            {
                auction.Title,
                auction.Description,
                auction.Price,
                auction.StartTime,
                auction.EndTime
            }
        );
        
        await connection.ExecuteAsync(command.sql, command.param);
    }

    public async Task<Auction> GetAuctionByIdAsync(int id, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = (
            sql: """
                SELECT id AS "Id", title AS "Title", description AS "Description", price AS "Price", start_time AS "StartTime", end_time AS "EndTime", is_sold AS "IsSold"
                FROM auctions.auctions
                WHERE id = @Id
            """,
            param: new { Id = id }
        );

        return await connection.QuerySingleAsync<Auction>(command.sql, command.param);
    }

    public async Task<IEnumerable<Auction>> GetAuctionsAsync(CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = (
            sql: """
                SELECT id AS "Id", title AS "Title", description AS "Description", price AS "Price", start_time AS "StartTime", end_time AS "EndTime", is_sold AS "IsSold"
                FROM auctions.auctions
            """,
            param: new { }
        );

        return await connection.QueryAsync<Auction>(command.sql, command.param);
    }

}