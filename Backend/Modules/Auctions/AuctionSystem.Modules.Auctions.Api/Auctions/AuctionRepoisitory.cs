using AuctionSystem.Modules.Auctions.Api.Database;
using System.Data;
using Dapper;

namespace AuctionSystem.Modules.Auctions.Api.Auctions;

public sealed class AuctionRepository(
    NpgsqlAuctionsDbConnectionFactory connectionFactory) : IAuctionRepository
{
    public async Task<int> AddAuctionAsync(Auction auction, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        using IDbTransaction tx = connection.BeginTransaction();

        int auctionId = await connection.QuerySingleAsync<int>(
            """
            INSERT INTO auctions.auctions
                (title, description, price, start_time, end_time, status, created_at, updated_at, is_deleted)
            VALUES
                (@Title, @Description, @Price, @StartTime, @EndTime, @Status, @CreatedAt, @UpdatedAt, @IsDeleted)
            RETURNING id
            """,
            new
            {
                auction.Title, auction.Description, auction.Price, auction.StartTime,
                auction.EndTime, auction.Status, auction.CreatedAt,
                auction.UpdatedAt, auction.IsDeleted
            },
            transaction: tx);
        Vehicle vehicle = auction.Vehicle;
        int vehicleId = await connection.QuerySingleAsync<int>(
            """
            INSERT INTO auctions.vehicles
                (make, model, year, mileage, base_price, engine_size, fuel_type, vehicle_type, auction_id)
            VALUES
                (@Make, @Model, @Year, @Mileage, @BasePrice, @EngineSize, @FuelType, @VehicleType, @AuctionId)
            RETURNING id
            """,
            new
            {
                vehicle.Make, vehicle.Model, vehicle.Year, vehicle.Mileage, vehicle.BasePrice, vehicle.EngineSize,
                vehicle.FuelType, vehicle.VehicleType, AuctionId = auctionId
            },
            transaction: tx);

        switch (vehicle.VehicleDetails)
        {
            case PersonalCar p:
                await connection.ExecuteAsync(
                    """
                    INSERT INTO auctions.personal_cars
                        (vehicle_id, seat_count, isofix)
                    VALUES
                        (@VehicleId, @SeatCount, @Isofix)
                    """,
                    new {VehicleId = vehicleId, p.SeatCount, p.Isofix},
                    transaction: tx);
                break;
            case BusinessCar b:
                await connection.ExecuteAsync(
                    """
                    INSERT INTO auctions.business_cars
                        (vehicle_id, cargo_volume, max_payload)
                    VALUES
                        (@VehicleId, @CargoVolume, @MaxPayload)
                    """,
                    new { VehicleId = vehicleId, b.CargoVolume, b.MaxPayload },
                    transaction: tx);
                break;
            default:
                throw new InvalidOperationException("Unknown vehicle details type");
        }
        tx.Commit();
        return auctionId;
    }

    public async Task<Auction?> GetAuctionByIdAsync(int id, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = (
            sql: """
                SELECT id AS "Id", title AS "Title", description AS "Description", 
                price AS "Price", start_time AS "StartTime", end_time AS "EndTime", 
                status AS "Status", created_at AS "CreatedAt", updated_at AS "UpdatedAt", 
                is_deleted AS "IsDeleted"

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
                SELECT id AS "Id", title AS "Title", description AS "Description", 
                price AS "Price", start_time AS "StartTime", end_time AS "EndTime", 
                status AS "Status", created_at AS "CreatedAt", updated_at AS "UpdatedAt", 
                is_deleted AS "IsDeleted"

                FROM auctions.auctions
            """,
            param: new { }
        );

        return (IReadOnlyList<Auction>)await connection.QueryAsync<Auction>(command.sql, command.param);
    }

}