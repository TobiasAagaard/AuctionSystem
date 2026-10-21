using System.Data;
using Auctionly.Modules.Auctions.Domain.Auctions;
using Auctionly.Modules.Auctions.Domain.Vehicles;
using Auctionly.Modules.Auctions.Domain.Vehicles.ValueObjects;
using Auctionly.Modules.Auctions.Infrastructure.Database;
using Dapper;

namespace Auctionly.Modules.Auctions.Infrastructure.Auctions;

internal sealed class AuctionRepository(NpgsqlAuctionsDbConnectionFactory connectionFactory) : IAuctionRepository
{
    public async Task<int> AddAsync(Auction auction, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);
        using IDbTransaction transaction = connection.BeginTransaction();

        int auctionId = await connection.QuerySingleAsync<int>(new CommandDefinition(
            """
            INSERT INTO auctions.auctions
                (title, description, price, start_time, end_time, status, created_at, updated_at, is_deleted)
            VALUES
                (@Title, @Description, @Price, @StartTime, @EndTime, @Status, @CreatedAt, @UpdatedAt, @IsDeleted)
            RETURNING id
            """,
            new
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
            },
            transaction,
            cancellationToken: cancellationToken));

        Vehicle vehicle = auction.Vehicle;

        int vehicleId = await connection.QuerySingleAsync<int>(new CommandDefinition(
            """
            INSERT INTO auctions.vehicles
                (make, model, year, mileage, base_price, engine_size, fuel_type, vehicle_type, auction_id)
            VALUES
                (@Make, @Model, @Year, @Mileage, @BasePrice, @EngineSize, @FuelType, @VehicleType, @AuctionId)
            RETURNING id
            """,
            new
            {
                vehicle.Make,
                vehicle.Model,
                vehicle.Year,
                vehicle.Mileage,
                vehicle.BasePrice,
                vehicle.EngineSize,
                FuelType = vehicle.FuelType.ToString(),
                VehicleType = vehicle.VehicleType.ToString(),
                AuctionId = auctionId
            },
            transaction,
            cancellationToken: cancellationToken));

        await InsertSpecificationsAsync(connection, transaction, vehicleId, vehicle.VehicleDetails, cancellationToken);

        transaction.Commit();

        return auctionId;
    }

    private static Task<int> InsertSpecificationsAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        int vehicleId,
        VehicleSpecifications specifications,
        CancellationToken cancellationToken)
    {
        CommandDefinition command = specifications switch
        {
            PersonalCarSpecifications p => new CommandDefinition(
                """
                INSERT INTO auctions.personal_cars (vehicle_id, seat_count, isofix)
                VALUES (@VehicleId, @SeatCount, @Isofix)
                """,
                new { VehicleId = vehicleId, p.SeatCount, p.Isofix },
                transaction,
                cancellationToken: cancellationToken),

            BusinessCarSpecifications b => new CommandDefinition(
                """
                INSERT INTO auctions.business_cars (vehicle_id, cargo_volume, max_payload)
                VALUES (@VehicleId, @CargoVolume, @MaxPayload)
                """,
                new { VehicleId = vehicleId, b.CargoVolume, b.MaxPayload },
                transaction,
                cancellationToken: cancellationToken),

            _ => throw new InvalidOperationException($"Unknown vehicle specifications '{specifications.GetType().Name}'.")
        };

        return connection.ExecuteAsync(command);
    }
}
