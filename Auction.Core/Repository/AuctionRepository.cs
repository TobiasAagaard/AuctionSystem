using Auction_Core.Models;
using Npgsql;

namespace Auction_Core.Repository;

public delegate void NotificationDelegate(IAuction auction, decimal bid);

public class AuctionRepository : IAuctionRepository
{
    private readonly Database _database;
    private UserRepository _userRepository;
    private VehicleRepository _vehicleRepository;

    public AuctionRepository(Database database)
    {
        _database = database;
        _userRepository = new UserRepository(database);
        _vehicleRepository = new VehicleRepository(database);
    }

    // Auction-related methods

    public async Task<int> AddAuctionAsync(Vehicle vehicle, ISeller seller, decimal minimumPrice, DateTime endTime, NotificationDelegate? notificationFunction)
    {
        using NpgsqlConnection connection = await _database.GetConnection();

        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO auctions (seller_id, vehicle_id, minimum_price, end_time)
                            VALUES (@seller_id, @vehicle_id, @minimum_price, @end_time)";
        
        cmd.Parameters.AddWithValue("seller_id", seller.ID);
        cmd.Parameters.AddWithValue("vehicle_id", vehicle.Id);
        cmd.Parameters.AddWithValue("minimum_price", minimumPrice);
        cmd.Parameters.AddWithValue("end_time", endTime);
        
        object? result = await cmd.ExecuteScalarAsync();

        if (result != null && result != DBNull.Value)
        {
            return (int)result;
        }
        
        return 0;
    }

    public async Task<int> AddAuctionAsync(Vehicle vehicle, ISeller seller, decimal minimumPrice, DateTime endTime)
    {
        return await AddAuctionAsync(vehicle, seller, minimumPrice, endTime, null);
    }

    public async Task<IEnumerable<Auction>> GetAllAuctionsAsync()
    {
        IEnumerable<Auction> auctions = new List<Auction>();
        using NpgsqlConnection connection = await _database.GetConnection();

        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, seller_id, vehicle_id, minimum_price, end_time, created_at, updated_at FROM auctions";

        using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (reader.Read())
            auctions = auctions.Append(await ReadAuctionFromReader(reader));

        return auctions;
    }

    public async Task<Auction> GetAuctionByIdAsync(int auctionId)
    {
        using NpgsqlConnection connection = await _database.GetConnection();

        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, seller_id, vehicle_id, minimum_price, end_time, created_at, updated_at FROM auctions WHERE id = @id";

        cmd.Parameters.AddWithValue("id", auctionId);

        using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (reader.Read())
        {
            return await ReadAuctionFromReader(reader);
        }

        throw new KeyNotFoundException("This auction does not exist.");
    }
    

    public async Task<bool> RemoveAuctionAsync(int auctionId)
    {
        using NpgsqlConnection connection = await _database.GetConnection();

        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM auctions WHERE id = @id";

        cmd.Parameters.AddWithValue("id", auctionId);

        int rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAuctionAsync(IAuction auction)
    {
        using NpgsqlConnection connection = await _database.GetConnection();

        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"UPDATE auctions 
                            SET seller_id       = @seller_id,
                                vehicle_id      = @vehicle_id,
                                minimum_price   = @minimum_price,
                                end_time        = @end_time,
                                created_at      = @created_at,
                                updated_at      = @updated_at
                            WHERE id = @id";

        BindAuctionToCommand(cmd, auction);

        int rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    // Bid-related methods

    public async Task<bool> AddBidAsync(int auctionId, IBuyer buyer, decimal bidAmount)
    {
        using var connection = await _database.GetConnection();
        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "INSERT INTO bids (buyer_id, auction_id, amount) VALUES (@buyerId, @auctionId, @amount)";

        cmd.Parameters.AddWithValue("@buyerId", buyer.ID);
        cmd.Parameters.AddWithValue("@auctionId", auctionId);
        cmd.Parameters.AddWithValue("@amount", bidAmount);

        int rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Bid>> GetBidsByAuctionIdAsync(int auctionId)
    {
        var bids = new List<Bid>();

        using var connection = await _database.GetConnection();
        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, buyer_id, auction_id, amount, created_at FROM bids WHERE auction_id = @auctionId";
        cmd.Parameters.AddWithValue("@auctionId", auctionId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            bids.Add(await ReadBidFromReader(reader));
        }

        return bids;
    }

    public async Task<Bid?> GetHighestBidByAuctionIdAsync(int auctionId)
    {
        using var connection = await _database.GetConnection();
        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, buyer_id, auction_id, amount, created_at FROM bids WHERE auction_id = @auctionId ORDER BY amount DESC LIMIT 1";
        cmd.Parameters.AddWithValue("@auctionId", auctionId);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return await ReadBidFromReader(reader);
        }

        return null; // Or throw an exception
        throw new KeyNotFoundException("No bids found for this auction.");
    }

    // Helper methods to read Auction and Bid from the database

    private async Task<Auction> ReadAuctionFromReader(NpgsqlDataReader reader)
    {
        var seller = await _userRepository.GetUserByIdAsync(reader.GetInt32(1));
        Vehicle vehicle = await _vehicleRepository.GetVehicleByIdAsync(reader.GetInt32(2));

        return new Auction
        (
            reader.GetInt32(0),
            seller,
            vehicle,
            reader.GetDecimal(3),
            reader.GetDateTime(4),
            reader.GetDateTime(5),
            reader.GetDateTime(6),
            null
        );
    }
    
    private static void BindAuctionToCommand(NpgsqlCommand cmd, IAuction auction)
    {
        cmd.Parameters.AddWithValue("id", auction.Id);
        cmd.Parameters.AddWithValue("seller_id", auction.Seller.ID);
        cmd.Parameters.AddWithValue("vehicle_id", auction.Vehicle.Id);
        cmd.Parameters.AddWithValue("minimum_price", auction.MinimumPrice);
        cmd.Parameters.AddWithValue("created_at", auction.CreatedAt);
        cmd.Parameters.AddWithValue("updated_at", auction.UpdatedAt);
    }

    private async Task<Bid> ReadBidFromReader(NpgsqlDataReader reader)
    {
        var bidder = await _userRepository.GetUserByIdAsync(reader.GetInt32(1));
        var auction = await GetAuctionByIdAsync(reader.GetInt32(2));

        return new Bid
        (
            reader.GetInt32(0),
            bidder,
            auction,
            reader.GetDecimal(3),
            reader.GetDateTime(4)
        );
    }
}
