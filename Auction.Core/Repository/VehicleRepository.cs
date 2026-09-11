using System.Data.Common;
using Auction_Core.Enums;
using Auction_Core.Models;
using Auction_Core.Utilities;
using Npgsql;


namespace Auction_Core.Repository;

public class VehicleRepository : IVehicleRepository
{
    private readonly Database _database;

    public VehicleRepository(Database database)
    {
        _database = database;
    }

    private const string InsertVehicle = """
        WITH new_vehicle AS (
            INSERT INTO vehicles (name, kilometers, release_year, registration_number, base_price,
                                  tow_bar, engine_size, km_per_liter, fuel_type)
            VALUES (@name, @kilometers, @release_year, @registration_number, @base_price,
                    @tow_bar, @engine_size, @km_per_liter, @fuel_type::FuelType)
            RETURNING id
        )
        """;

    private const string InsertHeavyVehicle = """
        new_heavy_vehicle AS (
            INSERT INTO heavy_vehicles (vehicle_id, weight, height, length)
            VALUES ((SELECT id FROM new_vehicle), @weight, @height, @length)
            RETURNING id
        )
        """;

    private const string InsertPersonalCar = """
        new_personal_car AS (
            INSERT INTO personal_cars (vehicle_id, seat_count)
            VALUES ((SELECT id FROM new_vehicle), @seat_count)
            RETURNING id
        )
        """;

    public async Task<Vehicle> GetVehicleByIdAsync(int id)
    {
        await using var connection = await _database.GetConnection();
        await using var command = new NpgsqlCommand("""
             SELECT  v.id, v.name, v.release_year, v.registration_number, v.base_price, v.tow_bar, v.engine_size,
                v.kilometers, v.km_per_liter, v.fuel_type,
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
            WHERE v.id = @id;
            """, connection);
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
        if (vehicle is null)
        {
            throw new ArgumentNullException(nameof(vehicle), "Vehicle cannot be null.");
        }

        await using var connection = await _database.GetConnection();
        await using var command = new NpgsqlCommand
        {
            Connection = connection
        };

        AddSharedParameters(command, vehicle);
        command.CommandText = BuildSubTypeInsert(command, vehicle);

        object id = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException($"Inserting vehicle '{vehicle.Name}' did not return a generated id.");
        vehicle.Id = Convert.ToInt32(id);
    }


    public async Task UpdateVehicleAsync(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteVehicleAsync(int id)
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
        FuelType FuelType);



     private static string BuildSubTypeInsert(NpgsqlCommand command, Vehicle vehicle)
    {
        if (vehicle is SemiTruck semiTruck)
        {
            AddHeavyVehicleParameters(command, semiTruck);
            command.Parameters.AddWithValue("@cargo_capacity", semiTruck.MaxLoad);
            return $"""
                {InsertVehicle},
                {InsertHeavyVehicle},
                new_semi_truck AS (
                    INSERT INTO semi_trucks (heavy_vehicle_id, cargo_capacity)
                    VALUES ((SELECT id FROM new_heavy_vehicle), @cargo_capacity)
                )
                SELECT id FROM new_vehicle
                """;
        }

        if (vehicle is Bus bus)
        {
            AddHeavyVehicleParameters(command, bus);
            command.Parameters.AddWithValue("@seat_count", bus.Seats);
            command.Parameters.AddWithValue("@bed_count", bus.SleepingPlaces);
            command.Parameters.AddWithValue("@toilet", bus.HasToilet);
            return $"""
                {InsertVehicle},
                {InsertHeavyVehicle},
                new_bus AS (
                    INSERT INTO buses (heavy_vehicle_id, seat_count, bed_count, toilet)
                    VALUES ((SELECT id FROM new_heavy_vehicle), @seat_count, @bed_count, @toilet)
                )
                SELECT id FROM new_vehicle
                """;
        }

        if (vehicle is BusinessPersonalCar businessPersonalCar)
        {
            command.Parameters.AddWithValue("@seat_count", businessPersonalCar.SeatCount);
            command.Parameters.AddWithValue("@cargo_capacity", businessPersonalCar.CargoCapacity);
            command.Parameters.AddWithValue("@roll_cage", businessPersonalCar.RollCage);
            return $"""
                {InsertVehicle},
                {InsertPersonalCar},
                new_business_personal_car AS (
                    INSERT INTO business_personal_cars (car_id, cargo_capacity, roll_cage)
                    VALUES ((SELECT id FROM new_personal_car), @cargo_capacity, @roll_cage)
                )
                SELECT id FROM new_vehicle
                """;
        }

        if (vehicle is PrivatePersonalCar privatePersonalCar)
        {
            command.Parameters.AddWithValue("@seat_count", privatePersonalCar.SeatCount);
            command.Parameters.AddWithValue("@isofix", privatePersonalCar.Isofix);
            return $"""
                {InsertVehicle},
                {InsertPersonalCar},
                new_private_personal_car AS (
                    INSERT INTO private_personal_cars(car_id, isofix)
                    VALUES ((SELECT id FROM new_personal_car), @isofix)
                )
                SELECT id FROM new_vehicle
                """;
        }

        throw new InvalidOperationException($"Unsupported vehicle type: {vehicle.GetType().Name}");
    }
    private static Vehicle MapVehicle(DbDataReader reader)
    {
        VehicleRow row = ReadSharedColumns(reader);

        if (reader.HasValue("truck_cargo_capacity"))
        {
            return MapSemiTruck(reader, row);
        }

        if (reader.HasValue("bus_seat_count"))
        {
            return MapBus(reader, row);
        }

        if (reader.HasValue("business_cargo_capacity"))
        {
            return MapBusinessPersonalCar(reader, row);
        }

        if (reader.HasValue("isofix"))
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
            Kilometers: reader.ReadDouble("kilometers"),
            RegistrationNumber: reader.GetString(reader.GetOrdinal("registration_number")),
            Year: reader.GetInt32(reader.GetOrdinal("release_year")),
            BasePrice: reader.ReadDouble("base_price"),
            TowBar: reader.GetBoolean(reader.GetOrdinal("tow_bar")),
            EngineSize: reader.ReadDouble("engine_size"),
            KmPerLiter: reader.HasValue("km_per_liter") ? reader.ReadDouble("km_per_liter") : 0,
            FuelType: Enum.Parse<FuelType>(reader.GetString(reader.GetOrdinal("fuel_type")), true));
    }

    private static void AddSharedParameters(NpgsqlCommand command, Vehicle vehicle)
    {
        command.Parameters.AddWithValue("@name", vehicle.Name);
        command.Parameters.AddWithValue("@kilometers", vehicle.Kilometers);
        command.Parameters.AddWithValue("@release_year", vehicle.Year);
        command.Parameters.AddWithValue("@registration_number", vehicle.RegistrationNumber);
        command.Parameters.AddWithValue("@base_price", (decimal)vehicle.BasePrice);
        command.Parameters.AddWithValue("@tow_bar", vehicle.TowBar);
        command.Parameters.AddWithValue("@engine_size", vehicle.EngineSize);
        command.Parameters.AddWithValue("@km_per_liter", vehicle.KmPerLiter);

        command.Parameters.AddWithValue("@fuel_type", vehicle.FuelType.ToString());
    }

    private static void AddHeavyVehicleParameters(NpgsqlCommand command, HeavyVehicle vehicle)
    {
        command.Parameters.AddWithValue("@weight", vehicle.Weight);
        command.Parameters.AddWithValue("@height", vehicle.Height);
        command.Parameters.AddWithValue("@length", vehicle.Length);
    }

    private static SemiTruck MapSemiTruck(DbDataReader reader, VehicleRow row)
    {
        return new SemiTruck(
            row.Id, row.Name, row.Kilometers, row.RegistrationNumber, row.Year, row.BasePrice,
            row.TowBar, row.EngineSize, row.KmPerLiter,
            maxLoad: reader.ReadDouble("truck_cargo_capacity"),
            height: reader.ReadDouble("height"),
            weight: reader.ReadDouble("weight"),
            length: reader.ReadDouble("length"));
    }

    private static Bus MapBus(DbDataReader reader, VehicleRow row)
    {
        return new Bus(
            row.Id, row.Name, row.Kilometers, row.RegistrationNumber, row.Year, row.BasePrice,
            row.TowBar, row.EngineSize, row.KmPerLiter,
            weight: reader.ReadDouble("weight"),
            height: reader.ReadDouble("height"),
            length: reader.ReadDouble("length"),
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
            cargoCapacity: reader.ReadDouble("business_cargo_capacity"));
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
