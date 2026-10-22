using MediatR;
using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using System.Data.Common;
using Dapper;
using Auctionly.Modules.Auctions.Domain.Vehicles;

namespace Auctionly.Modules.Auctions.Application.Auctions.GetAuction;

internal sealed class GetAuctionQueryHandler (IDbConnectionFactory dbConnectionFactory) : IRequestHandler<GetAuctionQuery, GetAuctionResponse?>
{
    public async Task<GetAuctionResponse?> Handle(GetAuctionQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);

            const string sql =
            """
            SELECT
                a.id            AS AuctionId,
                a.title         AS Title,
                a.description   AS Description,
                a.price         AS StartingPrice,
                a.start_time    AS StartTime,
                a.end_time      AS EndTime,
                v.id            AS VehicleId,
                v.make          AS Make,
                v.model         AS Model,
                v.year          AS Year,
                v.mileage       AS Mileage,
                v.base_price    AS PurchasePrice,
                v.engine_size   AS EngineSize,
                v.fuel_type     AS FuelType,
                v.vehicle_type  AS VehicleType,
                pc.seat_count   AS SeatCount,
                pc.isofix       AS Isofix,
                bc.cargo_volume AS CargoVolume,
                bc.max_payload  AS MaxPayload
            FROM auctions.auctions a
            JOIN auctions.vehicles v            ON v.auction_id = a.id
            LEFT JOIN auctions.personal_cars pc ON pc.vehicle_id = v.id
            LEFT JOIN auctions.business_cars bc ON bc.vehicle_id = v.id
            WHERE a.id = @AuctionId AND a.is_deleted = FALSE
            """;

            AuctionRows? auction = await connection.QuerySingleOrDefaultAsync<AuctionRows>(
            new CommandDefinition(sql, new { request.AuctionId }, cancellationToken: cancellationToken));

        return auction?.ToResponse();
    }

    private sealed record AuctionRows(
    int AuctionId, string Title, string Description, decimal StartingPrice,
    DateTime StartTime, DateTime EndTime,
    int VehicleId, string Make, string Model, int Year, int Mileage,
    decimal PurchasePrice, double EngineSize, string FuelType, string VehicleType,
    int? SeatCount, bool? Isofix, double? CargoVolume, double? MaxPayload)
    {

        public GetAuctionResponse ToResponse()
        {
            VehicleSpecificationResponse spec = VehicleType switch
            {
                "PersonalCar" => new PersonalCarResponse(SeatCount!.Value, Isofix!.Value),
                "BusinessCar" => new BusinessCarResponse(CargoVolume!.Value, MaxPayload!.Value),
                _ => throw new InvalidOperationException($"Unknown vehicle type '{VehicleType}'.")
            };

            return new GetAuctionResponse(
                AuctionId, Title, Description, StartingPrice, StartTime, EndTime,
                new GetAuctionVehicleResponse(
                    VehicleId, Make, Model, Year, Mileage, PurchasePrice, EngineSize,
                    Enum.Parse<FuelType>(FuelType), spec));
        }
    }

}