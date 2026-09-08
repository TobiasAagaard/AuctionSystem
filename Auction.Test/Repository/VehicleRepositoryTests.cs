using Auction_Core.Enums;
using Auction_Core.Models;
using Auction_Core.Repository;
using Npgsql;

namespace Auction_Test.Repository;
public class VehicleRepositoryTests : IAsyncLifetime
{
    private readonly Database _database = new();
    private readonly VehicleRepository _vehicleRepository;

    private string? _skipReason;
    private int _vehicleId;
    private int _personalCarId;

    private readonly PrivatePersonalCar _expectedVehicle = new(
        id: 0, // Replaced with the generated id once the row is inserted.
        name: "Test Vehicle",
        kilometers: 1000,
        registrationNumber: $"T{Random.Shared.Next(0, 999_999_999):D9}",
        year: 2020,
        basePrice: 20000,
        towBar: true,
        licenseType: LicenseType.B,
        engineSize: 2.0,
        kmPerLiter: 15,
        fuelType: FuelType.Petrol,
        seatCount: 5,
        isofix: true);

    public VehicleRepositoryTests()
    {
        _vehicleRepository = new VehicleRepository(_database);
    }

    public async ValueTask InitializeAsync()
    {
        NpgsqlConnection connection;
        try
        {
            connection = await _database.GetConnection();
        }
        catch (Exception exception) when (exception is NpgsqlException or System.Net.Sockets.SocketException)
        {
            _skipReason = $"No test database reachable ({exception.Message}). " +
                          "Start it with 'docker compose up -d database' or set AUCTION_TEST_DB.";
            return;
        }

        await using (connection)
        {
            _vehicleId = await InsertVehicleAsync(connection);
            _personalCarId = await InsertPersonalCarAsync(connection, _vehicleId);
            await InsertPrivatePersonalCarAsync(connection, _personalCarId);
        }

        _expectedVehicle.Id = _vehicleId;
    }

    public async ValueTask DisposeAsync()
    {
        if (_vehicleId == 0)
        {
            return;
        }

        await using var connection = await _database.GetConnection();
        await ExecuteAsync(connection, "DELETE FROM private_personal_cars WHERE car_id = @id", ("@id", _personalCarId));
        await ExecuteAsync(connection, "DELETE FROM personal_cars WHERE id = @id", ("@id", _personalCarId));
        await ExecuteAsync(connection, "DELETE FROM vehicles WHERE id = @id", ("@id", _vehicleId));
    }

    [Fact]
    public async Task GetVehicleByIdAsync_ReturnsTheSeededPrivatePersonalCar()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);

        // Act
        Vehicle? result = await _vehicleRepository.GetVehicleByIdAsync(_vehicleId);

        // Assert
        Assert.NotNull(result);
        PrivatePersonalCar car = Assert.IsType<PrivatePersonalCar>(result);

        Assert.Equal(_expectedVehicle.Id, car.Id);
        Assert.Equal(_expectedVehicle.Name, car.Name);
        Assert.Equal(_expectedVehicle.Kilometers, car.Kilometers);
        Assert.Equal(_expectedVehicle.RegistrationNumber, car.RegistrationNumber);
        Assert.Equal(_expectedVehicle.Year, car.Year);
        Assert.Equal(_expectedVehicle.BasePrice, car.BasePrice);
        Assert.Equal(_expectedVehicle.TowBar, car.TowBar);
        Assert.Equal(_expectedVehicle.EngineSize, car.EngineSize);
        Assert.Equal(_expectedVehicle.KmPerLiter, car.KmPerLiter);
        Assert.Equal(_expectedVehicle.FuelType, car.FuelType);
        Assert.Equal(_expectedVehicle.LicenseType, car.LicenseType);
        Assert.Equal(_expectedVehicle.SeatCount, car.SeatCount);
        Assert.Equal(_expectedVehicle.Isofix, car.Isofix);
    }

    [Fact]
    public async Task GetVehicleByIdAsync_ReturnsNullWhenVehicleDoesNotExist()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);

        Vehicle? result = await _vehicleRepository.GetVehicleByIdAsync(-1);

        Assert.Null(result);
    }

    private async Task<int> InsertVehicleAsync(NpgsqlConnection connection)
    {
        const string sql = """
            INSERT INTO vehicles (name, kilometers, release_year, registration_number, base_price,
                                  tow_bar, engine_size, km_per_liter, licence_type, fuel_type)
            VALUES (@name, @kilometers, @year, @registrationNumber, @basePrice,
                    @towBar, @engineSize, @kmPerLiter, CAST(@licenceType AS LicenceType), CAST(@fuelType AS FuelType))
            RETURNING id
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@name", _expectedVehicle.Name);
        command.Parameters.AddWithValue("@kilometers", _expectedVehicle.Kilometers);
        command.Parameters.AddWithValue("@year", _expectedVehicle.Year);
        command.Parameters.AddWithValue("@registrationNumber", _expectedVehicle.RegistrationNumber);
        command.Parameters.AddWithValue("@basePrice", (decimal)_expectedVehicle.BasePrice);
        command.Parameters.AddWithValue("@towBar", _expectedVehicle.TowBar);
        command.Parameters.AddWithValue("@engineSize", _expectedVehicle.EngineSize);
        command.Parameters.AddWithValue("@kmPerLiter", _expectedVehicle.KmPerLiter);
        command.Parameters.AddWithValue("@licenceType", _expectedVehicle.LicenseType.ToString());
        command.Parameters.AddWithValue("@fuelType", _expectedVehicle.FuelType.ToString());

        return (int)(await command.ExecuteScalarAsync())!;
    }

    private async Task<int> InsertPersonalCarAsync(NpgsqlConnection connection, int vehicleId)
    {
        const string sql = """
            INSERT INTO personal_cars (seat_count, vehicle_id)
            VALUES (@seatCount, @vehicleId)
            RETURNING id
            """;

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@seatCount", _expectedVehicle.SeatCount);
        command.Parameters.AddWithValue("@vehicleId", vehicleId);

        return (int)(await command.ExecuteScalarAsync())!;
    }

    private async Task InsertPrivatePersonalCarAsync(NpgsqlConnection connection, int personalCarId)
    {
        const string sql = """
            INSERT INTO private_personal_cars (car_id, isofix)
            VALUES (@carId, @isofix)
            """;

        await ExecuteAsync(connection, sql, ("@carId", personalCarId), ("@isofix", _expectedVehicle.Isofix));
    }

    private static async Task ExecuteAsync(NpgsqlConnection connection, string sql, params (string Name, object Value)[] parameters)
    {
        await using var command = new NpgsqlCommand(sql, connection);
        foreach ((string name, object value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        await command.ExecuteNonQueryAsync();
    }
}
