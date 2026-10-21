using Auctionly.Modules.Auctions.Api.Auctions.Enums;
using Auctionly.Modules.Auctions.Api.Database;
using System.Data;
using Dapper;

namespace Auctionly.Modules.Auctions.Api.Auctions;

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
                auction.EndTime, Status = auction.Status.ToString(), auction.CreatedAt,
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
                FuelType = vehicle.FuelType.ToString(), VehicleType = vehicle.VehicleType.ToString(), AuctionId = auctionId
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
    private const string SelectAuctionWithVehicle = """
        SELECT
            a.id           AS "Id",
            a.title        AS "Title",
            a.description  AS "Description",
            a.price        AS "Price",
            a.status       AS "Status",
            a.start_time   AS "StartTime",
            a.end_time     AS "EndTime",

            v.id           AS "VehicleId",
            v.make         AS "Make",
            v.model        AS "Model",
            v.year         AS "Year",
            v.mileage      AS "Mileage",
            v.base_price   AS "BasePrice",
            v.engine_size  AS "EngineSize",
            v.fuel_type    AS "FuelType",
            v.vehicle_type AS "VehicleType",

            pc.seat_count   AS "SeatCount",
            pc.isofix       AS "Isofix",
            bc.cargo_volume AS "CargoVolume",
            bc.max_payload  AS "MaxPayload"
        FROM auctions.auctions a
        JOIN      auctions.vehicles      v  ON v.auction_id  = a.id
        LEFT JOIN auctions.personal_cars pc ON pc.vehicle_id = v.id
        LEFT JOIN auctions.business_cars bc ON bc.vehicle_id = v.id
        """;

    public async Task<GetAuctionResponse?> GetAuctionByIdAsync(int id, CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            SelectAuctionWithVehicle + """

                WHERE a.id = @Id AND a.is_deleted = FALSE
                """,
            new { Id = id },
            cancellationToken: cancellationToken);

        AuctionRow? row = await connection.QuerySingleOrDefaultAsync<AuctionRow>(command);

        return row is null ? null : ToResponse(row);
    }

    public async Task<IReadOnlyList<GetAuctionResponse>> GetAuctionsAsync(CancellationToken cancellationToken)
    {
        using IDbConnection connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            SelectAuctionWithVehicle + """

                WHERE a.is_deleted = FALSE
                ORDER BY a.id
                """,
            cancellationToken: cancellationToken);

        IEnumerable<AuctionRow> rows = await connection.QueryAsync<AuctionRow>(command);

        return rows.Select(ToResponse).ToList();
    }

    private static GetAuctionResponse ToResponse(AuctionRow row)
    {
        // vehicle_type says which detail columns are filled, so .Value is safe here.
        // If the data is broken (e.g. PersonalCar without a personal_cars row) this throws instead of returning wrong data.
        VehicleDetailsResponse details = row.VehicleType switch
        {
            VehicleType.PersonalCar => new PersonalCarResponse(row.SeatCount!.Value, row.Isofix!.Value),
            VehicleType.BusinessCar => new BusinessCarResponse(row.CargoVolume!.Value, row.MaxPayload!.Value),
            _ => throw new InvalidOperationException($"Unknown vehicle type '{row.VehicleType}'.")
        };

        var vehicle = new VehicleResponse(
            row.VehicleId,
            row.Make,
            row.Model,
            row.Year,
            row.Mileage,
            row.BasePrice,
            row.EngineSize,
            row.FuelType,
            details);

        return new GetAuctionResponse(
            row.Id,
            row.Title,
            row.Description,
            row.Price,
            row.Status,
            row.StartTime,
            row.EndTime,
            vehicle);
    }
}
