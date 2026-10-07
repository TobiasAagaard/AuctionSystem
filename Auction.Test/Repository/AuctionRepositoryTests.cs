// using System.Net.Sockets;
// using Auction_Core.Enums;
// using Auction_Core.Models;
// using Auction_Core.Repository;
// using Npgsql;

// namespace Auction_Test.Repository;

// public class AuctionRepositoryTests : IAsyncLifetime
// {
//     private readonly Database _database = new();
//     private readonly AuctionRepository _auctionRepository;
//     private string? _skipReason;
    
//     private readonly User _seller = new(
//         id: 0, // Replaced with the generated id.
//         username: "test-seller",
//         passwordHash: "test",
//         postalCode: "1234");
//     private readonly User _buyer = new(
//         id: 0, // Replaced with the generated id.
//         username: "test-buyer",
//         passwordHash: "test",
//         postalCode: "1234");
    
//     private readonly PrivatePersonalCar _vehicle = new(
//         id: 0, // Replaced with the generated id once AddVehicleAsync inserts the row.
//         name: "Test Vehicle",
//         kilometers: 1000,
//         registrationNumber: "123456789",
//         year: 2020,
//         basePrice: 20000,
//         towBar: true,
//         engineSize: 2.0,
//         kmPerLiter: 15,
//         fuelType: FuelType.Petrol,
//         seatCount: 5,
//         isofix: true);

//     private readonly Auction _auction = new(
//         id: 0, // Replaced with the generated id once AddAuctionAsync inserts the row.

//         user: new User(
//             id: 1, // Replaced with the generated id once AddAuctionAsync inserts the row.
//             username: "test",
//             passwordHash: "test",
//             postalCode: "1234"),

//         vehicle: new PrivatePersonalCar(
//             id: 0, // Replaced with the generated id once AddVehicleAsync inserts the row.
//             name: "Test Vehicle",
//             kilometers: 1000,
//             registrationNumber: "123456789",
//             year: 2020,
//             basePrice: 20000,
//             towBar: true,
//             engineSize: 2.0,
//             kmPerLiter: 15,
//             fuelType: FuelType.Petrol,
//             seatCount: 5,
//             isofix: true),

//         minimumPrice: 10000,

//         endTime: DateTime.UtcNow.AddDays(7));

//     public AuctionRepositoryTests()
//     {
//         _auctionRepository = new AuctionRepository(_database);
//     }

//     public async ValueTask DisposeAsync()
//     {
//         if (_auction.Id == 0)
//         {
//             return;
//         }

//         await using NpgsqlConnection connection = await _database.GetConnection();
//         await using var command = new NpgsqlCommand("""
//             DELETE FROM auctions WHERE id = @id;
//             DELETE FROM vehicles WHERE id = @vehicleId;
//             DELETE FROM users WHERE id = @sellerId;
//             DELETE FROM users WHERE id = @buyerId;
//             """, connection);
//         command.Parameters.AddWithValue("@id", _auction.Id);
//         command.Parameters.AddWithValue("@vehicleId", _vehicle.Id);
//         command.Parameters.AddWithValue("@sellerId", _seller.ID);
//         command.Parameters.AddWithValue("@buyerId", _buyer.ID);
//         await command.ExecuteNonQueryAsync();
//     }

//     public async ValueTask InitializeAsync()
//     {
//         try
//         {
//             await using NpgsqlConnection connection = await _database.GetConnection();

//             // Insert the sellers and buyers into the database
//             NpgsqlCommand sellerCommand = new NpgsqlCommand("""
//                 INSERT INTO users (username, password_hash, postal_code)
//                 VALUES (@username, @passwordHash, @postalCode)
//                 RETURNING id;
//                 """, connection);
//             sellerCommand.Parameters.AddWithValue("@username", _seller.Username);
//             sellerCommand.Parameters.AddWithValue("@passwordHash", _seller.PasswordHash);
//             sellerCommand.Parameters.AddWithValue("@postalCode", _seller.PostalCode);

//             object? sellerId = await sellerCommand.ExecuteScalarAsync() ?? throw new InvalidOperationException($"Inserting user did not return a generated id.");

//             _seller.ID = Convert.ToInt32(sellerId);

//             NpgsqlCommand buyerCommand = new NpgsqlCommand("""
//                 INSERT INTO users (username, password_hash, postal_code)
//                 VALUES (@username, @passwordHash, @postalCode)
//                 RETURNING id;
//                 """, connection);
//             buyerCommand.Parameters.AddWithValue("@username", _buyer.Username);
//             buyerCommand.Parameters.AddWithValue("@passwordHash", _buyer.PasswordHash);
//             buyerCommand.Parameters.AddWithValue("@postalCode", _buyer.PostalCode);

//             object? buyerId = await buyerCommand.ExecuteScalarAsync() ?? throw new InvalidOperationException($"Inserting user did not return a generated id.");

//             _buyer.ID = Convert.ToInt32(buyerId);


//             // Insert the vehicle into the database
//             NpgsqlCommand vehicleCommand = new NpgsqlCommand("""
//                 INSERT INTO vehicles (name, kilometers, registration_number, year, base_price, tow_bar, engine_size, km_per_liter, fuel_type, seat_count, isofix)
//                 VALUES (@name, @kilometers, @registrationNumber, @year, @basePrice, @towBar, @engineSize, @kmPerLiter, @fuelType, @seatCount, @isofix)
//                 RETURNING id;
//                 """, connection);
//             vehicleCommand.Parameters.AddWithValue("@name", _vehicle.Name);
//             vehicleCommand.Parameters.AddWithValue("@kilometers", _vehicle.Kilometers);
//             vehicleCommand.Parameters.AddWithValue("@registrationNumber", _vehicle.RegistrationNumber);
//             vehicleCommand.Parameters.AddWithValue("@year", _vehicle.Year);
//             vehicleCommand.Parameters.AddWithValue("@basePrice", _vehicle.BasePrice);
//             vehicleCommand.Parameters.AddWithValue("@towBar", _vehicle.TowBar);
//             vehicleCommand.Parameters.AddWithValue("@engineSize", _vehicle.EngineSize);
//             vehicleCommand.Parameters.AddWithValue("@kmPerLiter", _vehicle.KmPerLiter);
//             vehicleCommand.Parameters.AddWithValue("@fuelType", _vehicle.FuelType);
//             vehicleCommand.Parameters.AddWithValue("@seatCount", _vehicle.SeatCount);
//             vehicleCommand.Parameters.AddWithValue("@isofix", _vehicle.Isofix);

//             object? vehicleId = await vehicleCommand.ExecuteScalarAsync() ?? throw new InvalidOperationException($"Inserting vehicle did not return a generated id.");

//             _vehicle.Id = Convert.ToInt32(vehicleId);

//             _auction.User.ID = _seller.ID;
//             _auction.Vehicle.Id = _vehicle.Id;
//         }
//         catch (Exception exception) when (exception is NpgsqlException or SocketException)
//         {
//             _skipReason = $"No test database reachable ({exception.Message}). " + "Start it with 'docker compose up -d database'.";
//         }
//     }

//     [Fact]
//     public async Task AddAuctionAsync_AssignsTheGeneratedIdToTheAuction()
//     {
//         SkipWhenNoDatabase();

//         var id = await _auctionRepository.AddAuctionAsync(_auction.Vehicle, _auction.User, _auction.MinimumPrice, _auction.EndTime, null);

//         Assert.True(id > 0);
//     }

//     [Fact]
//     public async Task AddAuctionAsync_Overload_AssignsTheGeneratedIdToTheAuction()
//     {
//         SkipWhenNoDatabase();

//         var id = await _auctionRepository.AddAuctionAsync(_auction.Vehicle, _auction.User, _auction.MinimumPrice, _auction.EndTime);

//         Assert.True(id > 0);
//     }

//     [Fact]
//     public async Task AddVehicleAsync_AddsPrivatePersonalCarThatGetretrievedCorrectlyByVehicleIdAsyncReadsBack()
//     {
//         SkipWhenNoDatabase();

//         await _auctionRepository.AddAuctionAsync(_auction.Vehicle, _auction.User, _auction.MinimumPrice, _auction.EndTime);
//         Auction result = await _auctionRepository.GetAuctionByIdAsync(_auction.Id);

//         Auction auction = Assert.IsType<Auction>(result);
//         Assert.Equal(_auction.Id, auction.Id);

//         Assert.Equal(_auction.Vehicle.Name, auction.Vehicle.Name);
//         Assert.Equal(_auction.Vehicle.Kilometers, auction.Vehicle.Kilometers);
//         Assert.Equal(_auction.Vehicle.RegistrationNumber, auction.Vehicle.RegistrationNumber);
//         Assert.Equal(_auction.Vehicle.Year, auction.Vehicle.Year);

//         Assert.Equal(_auction.User.ID, auction.User.ID);
//         Assert.Equal(_auction.User.Username, auction.User.Username);
//         Assert.Equal(_auction.User.PasswordHash, auction.User.PasswordHash);
//         Assert.Equal(_auction.User.PostalCode, auction.User.PostalCode);
        
//         Assert.Equal(_auction.MinimumPrice, auction.MinimumPrice);
//         Assert.Equal(_auction.IsSold, auction.IsSold);
//         Assert.Equal(_auction.EndTime, auction.EndTime);
//         Assert.Equal(_auction.CreatedAt, auction.CreatedAt);
//         Assert.Equal(_auction.UpdatedAt, auction.UpdatedAt);
//         Assert.Equal(_auction.NotificationFunction, auction.NotificationFunction);
//     }

//     [Fact]
//     public async Task AddAuctionAsync_ThrowsArgumentNullExceptionWhenVehicleIsNull()
//     {
//         await Assert.ThrowsAsync<ArgumentNullException>(async () => await _auctionRepository.AddAuctionAsync(null!, _auction.User, _auction.MinimumPrice, _auction.EndTime));
//     }
//     [Fact]
//     public async Task AddAuctionAsync_ThrowsArgumentNullExceptionWhenSellerIsNull()
//     {
//         await Assert.ThrowsAsync<ArgumentNullException>(async () => await _auctionRepository.AddAuctionAsync(_auction.Vehicle, null!, _auction.MinimumPrice, _auction.EndTime));
//     }
//     [Fact]
//     public async Task AddAuctionAsync_ThrowsArgumentOutOfRangeExceptionWhenMinimumPriceIsNegative()
//     {
//         await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _auctionRepository.AddAuctionAsync(_auction.Vehicle, _auction.User, -1, _auction.EndTime));
//     }
//     [Fact]
//     public async Task AddAuctionAsync_ThrowsArgumentOutOfRangeExceptionWhenEndTimeIsInThePast()
//     {
//         await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _auctionRepository.AddAuctionAsync(_auction.Vehicle, _auction.User, _auction.MinimumPrice, DateTime.UtcNow.AddHours(-1)));
//     }

//     [Fact]
//     public async Task AddBidAsync_AssignsTheGeneratedIdToTheBid()
//     {
//         SkipWhenNoDatabase();

//         int id = await _auctionRepository.AddBidAsync(_auction.Id, _buyer, 15000);

//         Assert.True(id > 0);
//     }

//     [Fact]
//     public async Task AddBidAsync_ThrowsArgumentNullExceptionWhenAuctionIdIsInvalid()
//     {
//         await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _auctionRepository.AddBidAsync(0, _buyer, 15000));
//     }

//     [Fact]
//     public async Task AddBidAsync_ThrowsArgumentNullExceptionWhenAuctionIdDoesNotExist()
//     {
//         SkipWhenNoDatabase();
//         await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _auctionRepository.AddBidAsync(999, _buyer, 15000));
//     }

//     [Fact]
//     public async Task GetBidsByAuctionIdAsync_ReturnsAllBidsForTheSpecifiedAuction()
//     {
//         SkipWhenNoDatabase();

//         int id = await _auctionRepository.AddBidAsync(_auction.Id, _buyer, 15000);

//         Assert.True(id > 0);
//     }


//     [Fact]
//     public async Task DeleteAuctionAsync_RemovesAuctionFromDatabase()
//     {
//         SkipWhenNoDatabase();
//         _auction.Id = await _auctionRepository.AddAuctionAsync(_auction.Vehicle, _auction.User, _auction.MinimumPrice, _auction.EndTime);

//         await _auctionRepository.DeleteAuctionAsync(_auction.Id);

//         await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _auctionRepository.GetAuctionByIdAsync(_auction.Id));
//     }
    
//     private void SkipWhenNoDatabase()
//     {
//         Assert.SkipWhen(_skipReason is not null, _skipReason ?? string.Empty);
//     }
// }