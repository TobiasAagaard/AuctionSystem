using Auction_Core.Models;
using Npgsql;
using Auction_Core.Enums;

namespace Auction_Core.Repository;

public delegate void NotificationDelegate(Auction auction, decimal bid);

public class AuctionRepository : IAuctionRepository
{
    private Database Database = new Database();
    private UserRepository UserRepository = new UserRepository();

    public async Task<bool> AddAuctionAsync(Vehicle vehicle, ISeller seller, decimal minimumPrice, NotificationDelegate? notificationFunction)
    {
        using NpgsqlConnection connection = Database.GetConnection();

        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO auctions (seller_id, vehicle_id, minimum_price)
                            VALUES (@seller_id, @vehicle_id, @minimum_price)";
        
        cmd.Parameters.AddWithValue("seller_id", seller.ID);
        cmd.Parameters.AddWithValue("vehicle_id", vehicle.Id);
        cmd.Parameters.AddWithValue("minimum_price", minimumPrice);

        return true;
    }

    public async Task<bool> AddAuctionAsync(Vehicle vehicle, ISeller seller, decimal minimumPrice)
    {
        return await AddAuctionAsync(vehicle, seller, minimumPrice, null);
    }

    public async Task<IEnumerable<Auction>> GetAllAuctionsAsync()
    {
        IEnumerable<Auction> auctions = new List<Auction>();
        using NpgsqlConnection connection = Database.GetConnection();

        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, seller_id, vehicle_id, minimum_price, created_at, updated_at FROM auctions";

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
            auctions = auctions.Append(ReadAuctionFromReader(reader));

        return auctions;
    }

    public async Task<Auction> GetAuctionByIdAsync(int auctionId)
    {
        using NpgsqlConnection connection = Database.GetConnection();

        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, seller_id, vehicle_id, minimum_price, created_at, updated_at FROM auctions WHERE id = @id";

        cmd.Parameters.AddWithValue("id", auctionId);

        using NpgsqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return ReadAuctionFromReader(reader);
        }

        throw new KeyNotFoundException("This auction does not exist.");
    }
    

    public async Task<bool> RemoveAuctionAsync(int auctionId)
    {
        using NpgsqlConnection connection = Database.GetConnection();

        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM auctions WHERE id = @id";

        cmd.Parameters.AddWithValue("id", auctionId);

        int rowsAffected = cmd.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    public async Task<bool> UpdateAuctionAsync(Auction auction)
    {
        using NpgsqlConnection connection = Database.GetConnection();

        NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"UPDATE auctions 
                            SET seller_id       = @seller_id,
                                vehicle_id      = @vehicle_id,
                                minimum_price   = @minimum_price,
                                created_at      = @created_at,
                                updated_at      = @updated_at
                            WHERE id = @id";

        BindAuctionToCommand(cmd, auction);

        int rowsAffected = cmd.ExecuteNonQuery();
        return rowsAffected > 0;
    }

    private Auction ReadAuctionFromReader(NpgsqlDataReader reader)
    {
        // This is a placeholder implementation. In a real application, you would retrieve the vehicle from the database using the vehicle_id.
        return new Auction
        (
            1,
            UserRepository.GetUserById(reader.GetInt32(1)),
            new PrivatePersonalCar(1,"Test Car",10000,"AB12345",2020,200000,true,2.0,15.0,FuelType.Petrol,2,true,LicenseType.B),
            reader.GetDecimal(3),
            reader.GetDateTime(4),
            reader.GetDateTime(5),
            null
        );

        // This can be uncommented and used when the VehicleRepository is implemented to fetch the vehicle from the database.
        // return new Auction
        // (
        //     reader.GetInt32(0),
        //     UserRepository.GetUserById(reader.GetInt32(1)),
        //     VehicleRepository.GetVehicleById(reader.GetInt32(2)),
        //     reader.GetDecimal(3),
        //     reader.GetDateTime(4),
        //     reader.GetDateTime(5),
        //     null
        // );
    }
    
    private static void BindAuctionToCommand(NpgsqlCommand cmd, Auction auction)
    {
        cmd.Parameters.AddWithValue("id", auction.Id);
        cmd.Parameters.AddWithValue("seller_id", auction.Seller.ID);
        cmd.Parameters.AddWithValue("vehicle_id", auction.Vehicle.Id);
        cmd.Parameters.AddWithValue("minimum_price", auction.MinimumPrice);
        cmd.Parameters.AddWithValue("created_at", auction.CreatedAt);
        cmd.Parameters.AddWithValue("updated_at", auction.UpdatedAt);
    }
}
