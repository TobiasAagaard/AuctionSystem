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

    public async Task<Vehicle> GetVehicleByIdAsync(int id)
    {
        await using var connection = await _database.GetConnection();
        await using var command = new NpgsqlCommand(SelectVehicleByIdSql, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            throw new KeyNotFoundException($"Vehicle with ID {id} not found.");
        }

        return MapVehicle(reader);
    }

    public async Task AddVehicleAsync(Vehicle vehicle)
    {
        await using var connection = await _database.GetConnection();
        await using var transaction = await connection.BeginTransactionAsync();
        await using var command = new NpgsqlCommand("""
            INSERT INTO vehicles (name, release_year, registration_number, base_price, tow_bar, engine_size, kilometers, km_per_liter, fuel_type, licence_type)
            VALUES (@name, @release_year, @registration_number, @base_price, @tow_bar, @engine_size, @kilometers, @km_per_liter,
                    CAST(@fuel_type AS FuelType), CAST(@licence_type AS LicenceType))
            RETURNING id;
        """, connection, transaction);

        command.Parameters.AddWithValue("@name", vehicle.Name);
        command.Parameters.AddWithValue("@release_year", vehicle.Year);
        command.Parameters.AddWithValue("@registration_number", vehicle.RegistrationNumber);
        command.Parameters.AddWithValue("@base_price", (decimal)vehicle.BasePrice);
        command.Parameters.AddWithValue("@tow_bar", vehicle.TowBar);
        command.Parameters.AddWithValue("@engine_size", vehicle.EngineSize);
        command.Parameters.AddWithValue("@kilometers", vehicle.Kilometers);
        command.Parameters.AddWithValue("@km_per_liter", vehicle.KmPerLiter);
        command.Parameters.AddWithValue("@fuel_type", vehicle.FuelType.ToString());
        command.Parameters.AddWithValue("@licence_type", vehicle.LicenseType.ToString());

        int vehicleId = Convert.ToInt32(await command.ExecuteScalarAsync());

            if (vehicle is SemiTruck semiTruck)
            {
                int semiTruckId = await InsertHeavyVehicleAsync(connection, transaction, vehicleId, semiTruck);
                await ExecuteAsync(connection, transaction,
                    "INSERT INTO semi_trucks (heavy_vehicle_id, cargo_capacity) VALUES (@id, @cargo_capacity)",
                    ("@id", semiTruckId), ("@cargo_capacity", semiTruck.MaxLoad));
            }
            if (vehicle is Bus bus)
            {
                int busId = await InsertHeavyVehicleAsync(connection, transaction, vehicleId, bus);
                await ExecuteAsync(connection, transaction,
                    "INSERT INTO buses (heavy_vehicle_id, seat_count, bed_count, toilet) VALUES (@id, @seat_count, @bed_count, @toilet)",
                    ("@id", busId), ("@seat_count", bus.Seats), ("@bed_count", bus.SleepingPlaces), ("@toilet", bus.HasToilet));
            }
            if (vehicle is BusinessPersonalCar businessCar)
            {
                int businessCarId = await InsertPersonalCarAsync(connection, transaction, vehicleId, businessCar);
                await ExecuteAsync(connection, transaction,
                    "INSERT INTO business_personal_cars (car_id, cargo_capacity, roll_cage) VALUES (@id, @cargo_capacity, @roll_cage)",
                    ("@id", businessCarId), ("@cargo_capacity", businessCar.CargoCapacity), ("@roll_cage", businessCar.RollCage));
            }
            if (vehicle is PrivatePersonalCar privateCar)
            {
                int privateCarId = await InsertPersonalCarAsync(connection, transaction, vehicleId, privateCar);
                await ExecuteAsync(connection, transaction,
                    "INSERT INTO private_personal_cars (car_id, isofix) VALUES (@id, @isofix)",
                    ("@id", privateCarId), ("@isofix", privateCar.Isofix));
            }


            if (vehicle is null)
            {
                throw new ArgumentNullException(nameof(vehicle), "Vehicle cannot be null.");
            }

            if (vehicle is not SemiTruck and not Bus and not BusinessPersonalCar and not PrivatePersonalCar)
            {
                throw new ArgumentException($"Unsupported vehicle type: {vehicle.GetType().Name}", nameof(vehicle));
            }

        await transaction.CommitAsync();
    }

    public Task UpdateVehicleAsync(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public Task DeleteVehicleAsync(int id)
    {
        throw new NotImplementedException();
    }
    

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


    private static async Task<int> InsertHeavyVehicleAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int vehicleId, HeavyVehicle vehicle)
    {
        const string sql = """
            INSERT INTO heavy_vehicles (vehicle_id, weight, height, length)
            VALUES (@vehicle_id, @weight, @height, @length)
            RETURNING id
            """;

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@vehicle_id", vehicleId);
        command.Parameters.AddWithValue("@weight", vehicle.Weight);
        command.Parameters.AddWithValue("@height", vehicle.Height);
        command.Parameters.AddWithValue("@length", vehicle.Length);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    private static async Task<int> InsertPersonalCarAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, int vehicleId, PersonalCar car)
    {
        const string sql = """
            INSERT INTO personal_cars (seat_count, vehicle_id)
            VALUES (@seat_count, @vehicle_id)
            RETURNING id
            """;

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@seat_count", car.SeatCount);
        command.Parameters.AddWithValue("@vehicle_id", vehicleId);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    private static async Task ExecuteAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql, params (string Name, object Value)[] parameters)
    {
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        foreach ((string name, object value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        await command.ExecuteNonQueryAsync();
    }
}