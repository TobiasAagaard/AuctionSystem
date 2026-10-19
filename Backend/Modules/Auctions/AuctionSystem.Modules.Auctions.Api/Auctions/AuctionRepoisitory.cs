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
                    (title, description, price, start_time, end_time, status, created_at, updated_at, is_deleted)
                VALUES
                    (@Title, @Description, @Price, @StartTime, @EndTime, @Status, @CreatedAt, @UpdatedAt, @IsDeleted)
                """,
            param: new
            {
                auction.Title,
                auction.Description,
                auction.Price,
                auction.StartTime,
                auction.EndTime,
                Status = auction.Status.ToString(),
                auction.CreatedAt,
                auction.UpdatedAt,
                auction.IsDeleted
            }
        );
        
        await connection.ExecuteAsync(command.sql, command.param);
    }

    public async Task<Auction?> GetAuctionByIdAsync(int id, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = (
            sql: """
                SELECT id AS "Id", title AS "Title", description AS "Description", price AS "Price", start_time AS "StartTime", end_time AS "EndTime", status AS "Status", created_at AS "CreatedAt", updated_at AS "UpdatedAt", is_deleted AS "IsDeleted"
                FROM auctions.auctions
                WHERE id = @Id
            """,
            param: new { Id = id }
        );

        return await connection.QuerySingleOrDefaultAsync<Auction>(command.sql, command.param);
    }

    public async Task<IReadOnlyList<Auction>> GetAuctionsAsync(CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = (
            sql: """
                SELECT id AS "Id", title AS "Title", description AS "Description", price AS "Price", start_time AS "StartTime", end_time AS "EndTime", status AS "Status", created_at AS "CreatedAt", updated_at AS "UpdatedAt", is_deleted AS "IsDeleted"
                FROM auctions.auctions
            """,
            param: new { }
        );

        return (IReadOnlyList<Auction>)await connection.QueryAsync<Auction>(command.sql, command.param);
    }

}