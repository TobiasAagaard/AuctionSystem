using System.Net.Sockets;
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

    private readonly PrivatePersonalCar _vehicle = new(
        id: 0, // Replaced with the generated id once AddVehicleAsync inserts the row.
        name: "Test Vehicle",
        kilometers: 1000,
        registrationNumber: $"T{Random.Shared.Next(0, 999_999_999):D9}",
        year: 2020,
        basePrice: 20000,
        towBar: true,
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
        try
        {
            await using NpgsqlConnection connection = await _database.GetConnection();
        }
        catch (Exception exception) when (exception is NpgsqlException or SocketException)
        {
            _skipReason = $"No test database reachable ({exception.Message}). " + "Start it with 'docker compose up -d database'.";
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_vehicle.Id == 0)
        {
            return;
        }

        await using NpgsqlConnection connection = await _database.GetConnection();
        await using var command = new NpgsqlCommand("""
            DELETE FROM private_personal_cars WHERE car_id IN (SELECT id FROM personal_cars WHERE vehicle_id = @id);
            DELETE FROM personal_cars WHERE vehicle_id = @id;
            DELETE FROM vehicles WHERE id = @id;
            """, connection);
        command.Parameters.AddWithValue("@id", _vehicle.Id);

        await command.ExecuteNonQueryAsync();
    }

    [Fact]
    public async Task AddVehicleAsync_AssignsTheGeneratedIdToTheVehicle()
    {
        SkipWhenNoDatabase();

        await _vehicleRepository.AddVehicleAsync(_vehicle);

        Assert.True(_vehicle.Id > 0);
    }

    [Fact]
    public async Task AddVehicleAsync_AddsPrivatePersonalCarThatGetretrievedCorrectlyByVehicleIdAsyncReadsBack()
    {
        SkipWhenNoDatabase();

        await _vehicleRepository.AddVehicleAsync(_vehicle);
        Vehicle result = await _vehicleRepository.GetVehicleByIdAsync(_vehicle.Id);

        PrivatePersonalCar car = Assert.IsType<PrivatePersonalCar>(result);
        Assert.Equal(_vehicle.Id, car.Id);
        Assert.Equal(_vehicle.Name, car.Name);
        Assert.Equal(_vehicle.Kilometers, car.Kilometers);
        Assert.Equal(_vehicle.RegistrationNumber, car.RegistrationNumber);
        Assert.Equal(_vehicle.Year, car.Year);
        Assert.Equal(_vehicle.BasePrice, car.BasePrice);
        Assert.Equal(_vehicle.TowBar, car.TowBar);
        Assert.Equal(_vehicle.EngineSize, car.EngineSize);
        Assert.Equal(_vehicle.KmPerLiter, car.KmPerLiter);
        Assert.Equal(_vehicle.FuelType, car.FuelType);
        Assert.Equal(_vehicle.LicenseType, car.LicenseType);
        Assert.Equal(_vehicle.SeatCount, car.SeatCount);
        Assert.Equal(_vehicle.Isofix, car.Isofix);
    }

    [Fact]
    public async Task AddVehicleAsync_ThrowsArgumentNullExceptionWhenVehicleIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _vehicleRepository.AddVehicleAsync(null!));
    }

    private void SkipWhenNoDatabase()
    {
        Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);
    }
}
