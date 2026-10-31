using Auctionly.Modules.Auctions.Application.Abstractions.Handlers;
using Auctionly.Modules.Auctions.Application.Abstractions.Data;
using System.Data.Common;
using Dapper;
using Auctionly.Modules.Auctions.Domain.Vehicles;
using Auctionly.Modules.Auctions.Domain.ValueObjects.Enums;
using Auctionly.Modules.Auctions.Domain.Abstractions;
using Auctionly.Modules.Auctions.Domain.Auctions;

namespace Auctionly.Modules.Auctions.Application.Auctions.GetAuction;

internal sealed class GetAuctionQueryHandler (IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetAuctionQuery, Result<GetAuctionResponse>>
{
    public async Task<Result<GetAuctionResponse>> Handle(GetAuctionQuery request, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync(ct);

            const string sql =
            """
            SELECT
                a.id            AS AuctionId,
                a.title         AS Title,
                a.description   AS Description,
                a.price         AS Price,
                a.currency      AS Currency,
                a.start_time    AS StartTime,
                a.end_time      AS EndTime,
                v.id            AS VehicleId,
                v.make          AS Make,
                v.model         AS Model,
                v.year          AS Year,
                v.mileage       AS Mileage,
                v.purchase_price AS PurchasePrice,
                v.currency      AS VehicleCurrency,
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

            AuctionItems? auction = await connection.QuerySingleOrDefaultAsync<AuctionItems>(
            new CommandDefinition(sql, new { request.AuctionId }, cancellationToken: ct));

        if (auction is null)
        {
            return Result.Failure<GetAuctionResponse>(AuctionErrors.NotFound(request.AuctionId));
        }

        return auction.ToResponse();
    }

    private sealed record AuctionItems(
    int AuctionId, string Title, string Description, decimal Price, string Currency,
    DateTime StartTime, DateTime EndTime,
    int VehicleId, string Make, string Model, int Year, int Mileage,
    decimal PurchasePrice, string VehicleCurrency, double EngineSize, string FuelType, string VehicleType,
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
                AuctionId, Title, Description, Price, Enum.Parse<Currency>(Currency), StartTime, EndTime,
                new GetAuctionVehicleResponse(
                    VehicleId, Make, Model, Year, Mileage, PurchasePrice, Enum.Parse<Currency>(VehicleCurrency), EngineSize,
                    Enum.Parse<FuelType>(FuelType), spec));
        }
    }

}