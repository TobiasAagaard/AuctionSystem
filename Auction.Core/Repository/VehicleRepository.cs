using System.Data.Common;
using Auction_Core.Enums;
using Auction_Core.Models;
using Npgsql;

namespace Auction_Core.Repository;

public class VehicleRepository : IVehicleRepository
{
    private readonly Database _database;

    public VehicleRepository(Database database)
    {
        _database = database;
    }

    private const string SelectVehicleSql = """
        SELECT  v.id, v.name, v.release_year, v.registration_number, v.base_price, v.tow_bar, v.engine_size,
                v.kilometers, v.km_per_liter, v.fuel_type, v.licence_type,

                hv.weight, hv.height, hv.length,

                st.cargo_capacity AS truck_cargo_capacity,

                b.seat_count AS bus_seat_count, b.bed_count, b.toilet,

                pc.seat_count AS car_seat_count,

                bpc.cargo_capacity AS business_cargo_capacity, bpc.roll_cage,

                ppc.isofix

        FROM vehicles v
        LEFT JOIN heavy_vehicles hv ON v.id = hv.vehicle_id
        LEFT JOIN semi_trucks st ON hv.id = st.heavy_vehicle_id
        LEFT JOIN buses b ON hv.id = b.heavy_vehicle_id
        LEFT JOIN personal_cars pc ON v.id = pc.vehicle_id
        LEFT JOIN business_personal_cars bpc ON pc.id = bpc.car_id
        LEFT JOIN private_personal_cars ppc ON pc.id = ppc.car_id
        """;

    private const string SelectVehicleByIdSql = $"""
        {SelectVehicleSql}
        WHERE v.id = @id
        """;

    public async Task<Vehicle?> GetVehicleByIdAsync(int id)
    {
        await using var connection = await _database.GetConnection();
        await using var command = new NpgsqlCommand(SelectVehicleByIdSql, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? MapVehicle(reader) : null;
    }

    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
    {
        await using var connection = await _database.GetConnection();
        await using var command = new NpgsqlCommand(SelectVehicleSql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        var vehicles = new List<Vehicle>();
        while (await reader.ReadAsync())
        {
            vehicles.Add(MapVehicle(reader));
        }

        return vehicles;
    }
    public async Task AddVehicleAsync(Vehicle vehicle)
    {
        
    }

    public Task UpdateVehicleAsync(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public Task DeleteVehicleAsync(int id)
    {
        throw new NotImplementedException();
    }
    
    // Struct representing a row from the vehicles table with shared columns
    private readonly record struct VehicleRow(
        int Id,
        string Name,
        double Kilometers,
        string RegistrationNumber,
        int Year,
        double BasePrice,
        bool TowBar,
        double EngineSize,
        double KmPerLiter,
        FuelType FuelType,
        LicenseType LicenseType);

    private static Vehicle MapVehicle(DbDataReader reader)
    {
        VehicleRow row = ReadSharedColumns(reader);

        if (!reader.IsDBNull(reader.GetOrdinal("truck_cargo_capacity")))
        {
            return MapSemiTruck(reader, row);
        }

        if (!reader.IsDBNull(reader.GetOrdinal("bus_seat_count")))
        {
            return MapBus(reader, row);
        }

        if (!reader.IsDBNull(reader.GetOrdinal("business_cargo_capacity")))
        {
            return MapBusinessPersonalCar(reader, row);
        }

        if (!reader.IsDBNull(reader.GetOrdinal("isofix")))
        {
            return MapPrivatePersonalCar(reader, row);
        }

        throw new InvalidOperationException(
            $"Vehicle {row.Id} has no row in any sub type table (semi_trucks, buses, business_personal_cars, private_personal_cars).");
    }

    private static VehicleRow ReadSharedColumns(DbDataReader reader)
    {
        return new VehicleRow(
            Id: reader.GetInt32(reader.GetOrdinal("id")),
            Name: reader.GetString(reader.GetOrdinal("name")),
            Kilometers: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("kilometers"))),
            RegistrationNumber: reader.GetString(reader.GetOrdinal("registration_number")),
            Year: reader.GetInt32(reader.GetOrdinal("release_year")),
            BasePrice: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("base_price"))),
            TowBar: reader.GetBoolean(reader.GetOrdinal("tow_bar")),
            EngineSize: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("engine_size"))),
            KmPerLiter: reader.IsDBNull(reader.GetOrdinal("km_per_liter")) ? 0 : Convert.ToDouble(reader.GetValue(reader.GetOrdinal("km_per_liter"))),
            FuelType: Enum.Parse<FuelType>(reader.GetString(reader.GetOrdinal("fuel_type")), true),
            LicenseType: Enum.Parse<LicenseType>(reader.GetString(reader.GetOrdinal("licence_type")), true));
    }

    private static SemiTruck MapSemiTruck(DbDataReader reader, VehicleRow row)
    {
        return new SemiTruck(
            row.Id, row.Name, row.Kilometers, row.RegistrationNumber, row.Year, row.BasePrice,
            row.TowBar, row.EngineSize, row.KmPerLiter,
            maxLoad: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("truck_cargo_capacity"))),
            height: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("height"))),
            weight: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("weight"))),
            length: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("length"))));
    }

    private static Bus MapBus(DbDataReader reader, VehicleRow row)
    {
        return new Bus(
            row.Id, row.Name, row.Kilometers, row.RegistrationNumber, row.Year, row.BasePrice,
            row.TowBar, row.EngineSize, row.KmPerLiter,
            weight: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("weight"))),
            height: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("height"))),
            length: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("length"))),
            seats: reader.GetInt32(reader.GetOrdinal("bus_seat_count")),
            sleepingPlaces: reader.GetInt32(reader.GetOrdinal("bed_count")),
            hasToilet: reader.GetBoolean(reader.GetOrdinal("toilet")));
    }

    private static BusinessPersonalCar MapBusinessPersonalCar(DbDataReader reader, VehicleRow row)
    {
        return new BusinessPersonalCar(
            row.Id, row.Name, row.Kilometers, row.RegistrationNumber, row.Year, row.BasePrice,
            row.EngineSize, row.KmPerLiter, row.FuelType,
            seatCount: reader.GetInt32(reader.GetOrdinal("car_seat_count")),
            rollCage: reader.GetBoolean(reader.GetOrdinal("roll_cage")),
            cargoCapacity: Convert.ToDouble(reader.GetValue(reader.GetOrdinal("business_cargo_capacity"))));
    }

    private static PrivatePersonalCar MapPrivatePersonalCar(DbDataReader reader, VehicleRow row)
    {
        return new PrivatePersonalCar(
            row.Id, row.Name, row.Kilometers, row.RegistrationNumber, row.Year, row.BasePrice,
            row.TowBar, row.EngineSize, row.KmPerLiter, row.FuelType,
            seatCount: reader.GetInt32(reader.GetOrdinal("car_seat_count")),
            isofix: reader.GetBoolean(reader.GetOrdinal("isofix")));
    }
}
