using Auction_Core.Models;
using Auction_Core.Utilities;
using Npgsql;

namespace Auction_Core.Repository;

public class UserRepository : IUserRepository 
{
    private readonly Database _database;

    public UserRepository(Database database)
    {
        _database = database;
    }

    public async Task<User> GetUserByIdAsync(int id) {

        using NpgsqlConnection connection = await _database.GetConnection();

        using NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT * FROM users WHERE id = @id";

        cmd.Parameters.AddWithValue("id", id);

        using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (reader.Read())
        {
            return new User(
                reader.GetInt32(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("username")),
                reader.GetString(reader.GetOrdinal("password_hash")),
                reader.GetString(reader.GetOrdinal("postal_code"))
            );
        }
        throw new InvalidOperationException($"User with ID {id} not found.");
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync() {

        using NpgsqlConnection connection = await _database.GetConnection();

        using NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"SELECT * FROM users";

        using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        List<User> users = new List<User>();
        while (reader.Read())
        {
            users.Add(new User(
                reader.GetInt32(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("username")),
                reader.GetString(reader.GetOrdinal("password_hash")),
                reader.GetString(reader.GetOrdinal("postal_code"))
            ));
        }

        return users;
    }

    public async Task<bool> AddUserAsync(string username, string password, string postalCode) {
        
        using NpgsqlConnection connection = await _database.GetConnection();

        using NpgsqlCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"INSERT INTO users (username, password_hash, postal_code) VALUES (@username, @password_hash, @postal_code)";

        cmd.Parameters.AddWithValue("username", username);
        cmd.Parameters.AddWithValue("password_hash", PasswordHasher.Hash(password));
        cmd.Parameters.AddWithValue("postal_code", postalCode);

        return await cmd.ExecuteNonQueryAsync() == 1;
    }

    public bool UpdateUser(User user) {
        throw new NotImplementedException();
    }

    public bool DeleteUser(int id) {
        throw new NotImplementedException();
    }
}