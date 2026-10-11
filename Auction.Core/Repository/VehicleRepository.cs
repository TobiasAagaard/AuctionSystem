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

    public async Task<Vehicle> GetVehicleByIdAsync(int id)
    {
        await using var connection = await _database.GetConnection();
        await using var command = new NpgsqlCommand("SELECT * FROM get_vehicle_by_id(@id)", connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            throw new KeyNotFoundException($"Vehicle with ID {id} not found.");
        }

        return MapVehicle(reader);
    }

    public async Task<int> AddVehicleAsync(Vehicle vehicle)
    {
        if (vehicle is null)
        {
            throw new ArgumentNullException(nameof(vehicle), "Vehicle cannot be null.");
        }

        await using var connection = await _database.GetConnection();
        await using var command = new NpgsqlCommand{ Connection = connection };
        AddSharedParameters(command, vehicle);
        BuildSubTypeInsert(command, vehicle);

        object id = await command.ExecuteScalarAsync() ?? throw new InvalidOperationException($"Inserting vehicle '{vehicle.Name}' did not return a generated id.");

        vehicle.Id = Convert.ToInt32(id);
        
        return vehicle.Id;
    }


    public async Task UpdateVehicleAsync(Vehicle vehicle)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteVehicleAsync(int id)
    {
        await using var connection = await _database.GetConnection();
        await using var command = new NpgsqlCommand
        {
            Connection = connection,
        };
        
        command.Parameters.AddWithValue("@id", id);
        command.CommandText = "SELECT delete_vehicle(@id)";

        bool deleted = (bool)(await command.ExecuteScalarAsync() ?? false);
        if (!deleted)
        {
            throw new KeyNotFoundException($"Vehicle with ID {id} not found");
        }
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

    private const string SharedArguments = """
        @name,
        @kilometers,
        @release_year,
        @registration_number,
        @base_price,
        @tow_bar,
        @engine_size,
        @km_per_liter,
        @fuel_type::fueltype
        """;

    private static void BuildSubTypeInsert(NpgsqlCommand command, Vehicle vehicle)
    {
        switch (vehicle)
        {
            case SemiTruck semiTruck:
                command.CommandText = $"""
                    SELECT add_semi_truck(
                        {SharedArguments},
                        @weight,
                        @height,
                        @length,
                        @cargo_capacity
                    )
                    """;

                AddHeavyVehicleParameters(command, semiTruck);
                command.Parameters.AddWithValue("@cargo_capacity", semiTruck.MaxLoad);
                break;

            case Bus bus:
                command.CommandText = $"""
                    SELECT add_bus(
                        {SharedArguments},
                        @weight,
                        @height,
                        @length,
                        @seat_count,
                        @bed_count,
                        @toilet
                    )
                    """;

                AddHeavyVehicleParameters(command, bus);
                command.Parameters.AddWithValue("@seat_count", bus.Seats);
                command.Parameters.AddWithValue("@bed_count", bus.SleepingPlaces);
                command.Parameters.AddWithValue("@toilet", bus.HasToilet);
                break;

            case BusinessPersonalCar businessPersonalCar:
                command.CommandText = $"""
                    SELECT add_business_personal_car(
                        {SharedArguments},
                        @seat_count,
                        @cargo_capacity,
                        @roll_cage
                    )
                    """;

                command.Parameters.AddWithValue("@seat_count", businessPersonalCar.SeatCount);
                command.Parameters.AddWithValue("@cargo_capacity", businessPersonalCar.CargoCapacity);
                command.Parameters.AddWithValue("@roll_cage", businessPersonalCar.RollCage);
                break;

            case PrivatePersonalCar privatePersonalCar:
                command.CommandText = $"""
                    SELECT add_private_personal_car(
                        {SharedArguments},
                        @seat_count,
                        @isofix
                    )
                    """;

                command.Parameters.AddWithValue("@seat_count", privatePersonalCar.SeatCount);
                command.Parameters.AddWithValue("@isofix", privatePersonalCar.Isofix);
                break;

            default:
                throw new InvalidOperationException($"Vehicle type {vehicle.GetType().Name} is not supported.");
        }
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
