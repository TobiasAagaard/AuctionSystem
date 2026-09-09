using System.Data.Common;
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

    private const string SelectUserSql = """
        SELECT  u.id, u.username, u.password_hash, u.postal_code, u.balance,

                bc.cvr, bc.credit,

                pc.cpr

        FROM users u
        LEFT JOIN business_customers bc ON u.id = bc.user_id
        LEFT JOIN private_customers pc ON u.id = pc.user_id
        """;

    private const string SelectUserByIdSql = $"""
        {SelectUserSql}
        WHERE u.id = @id
        """;

    public async Task<User> GetUserByIdAsync(int id) {

        await using NpgsqlConnection connection = await _database.GetConnection();

        await using NpgsqlCommand cmd = new NpgsqlCommand(SelectUserByIdSql, connection);
        cmd.Parameters.AddWithValue("id", id);

        await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            throw new InvalidOperationException($"User with ID {id} not found.");
        }

        return MapUser(reader);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync() {

        await using NpgsqlConnection connection = await _database.GetConnection();

        await using NpgsqlCommand cmd = new NpgsqlCommand(SelectUserSql, connection);

        await using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        List<User> users = new List<User>();
        while (await reader.ReadAsync())
        {
            users.Add(MapUser(reader));
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

    public async Task<PrivateCustomer> AddPrivateCustomerAsync(string username, string password, string postalCode, string cpr)
    {
        await using NpgsqlConnection connection = await _database.GetConnection();
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();

        string passwordHash = PasswordHasher.Hash(password);
        int userId = await InsertUserAsync(connection, transaction, username, passwordHash, postalCode);

        await using NpgsqlCommand cmd = new NpgsqlCommand(
            "INSERT INTO private_customers (user_id, cpr) VALUES (@user_id, @cpr)", connection, transaction);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("cpr", cpr);
        await cmd.ExecuteNonQueryAsync();

        await transaction.CommitAsync();

        return new PrivateCustomer(userId, username, passwordHash, postalCode, cpr);
    }

    public async Task<BusinessCustomer> AddBusinessCustomerAsync(string username, string password, string postalCode, string cvr, decimal credit)
    {
        await using NpgsqlConnection connection = await _database.GetConnection();
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();

        string passwordHash = PasswordHasher.Hash(password);
        int userId = await InsertUserAsync(connection, transaction, username, passwordHash, postalCode);

        await using NpgsqlCommand cmd = new NpgsqlCommand(
            "INSERT INTO business_customers (user_id, cvr, credit) VALUES (@user_id, @cvr, @credit)", connection, transaction);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("cvr", cvr);
        cmd.Parameters.AddWithValue("credit", credit);
        await cmd.ExecuteNonQueryAsync();

        await transaction.CommitAsync();

        return new BusinessCustomer(userId, username, passwordHash, postalCode, credit, cvr);
    }

    private static async Task<int> InsertUserAsync(
        NpgsqlConnection connection, NpgsqlTransaction transaction, string username, string passwordHash, string postalCode)
    {
        await using NpgsqlCommand cmd = new NpgsqlCommand(
            "INSERT INTO users (username, password_hash, postal_code) VALUES (@username, @password_hash, @postal_code) RETURNING id",
            connection, transaction);

        cmd.Parameters.AddWithValue("username", username);
        cmd.Parameters.AddWithValue("password_hash", passwordHash);
        cmd.Parameters.AddWithValue("postal_code", postalCode);

        return (int)(await cmd.ExecuteScalarAsync() ?? throw new InvalidOperationException("Failed to insert user."));
    }

    public Task<bool> UpdateUserAsync(User user) {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserAsync(int id) {
        throw new NotImplementedException();
    }

    private readonly record struct UserRow(
        int Id,
        string Username,
        string PasswordHash,
        string PostalCode,
        decimal Balance);

    private static User MapUser(DbDataReader reader)
    {
        UserRow row = ReadSharedColumns(reader);

        if (!reader.IsDBNull(reader.GetOrdinal("cvr")))
        {
            return new BusinessCustomer(
                row.Id, row.Username, row.PasswordHash, row.PostalCode,
                credit: reader.GetDecimal(reader.GetOrdinal("credit")),
                cvr: reader.GetString(reader.GetOrdinal("cvr")))
            {
                Balance = row.Balance
            };
        }

        if (!reader.IsDBNull(reader.GetOrdinal("cpr")))
        {
            return new PrivateCustomer(
                row.Id, row.Username, row.PasswordHash, row.PostalCode,
                cpr: reader.GetString(reader.GetOrdinal("cpr")))
            {
                Balance = row.Balance
            };
        }

        return new User(row.Id, row.Username, row.PasswordHash, row.PostalCode)
        {
            Balance = row.Balance
        };
    }

    private static UserRow ReadSharedColumns(DbDataReader reader)
    {
        return new UserRow(
            Id: reader.GetInt32(reader.GetOrdinal("id")),
            Username: reader.GetString(reader.GetOrdinal("username")),
            PasswordHash: reader.GetString(reader.GetOrdinal("password_hash")),
            PostalCode: reader.GetString(reader.GetOrdinal("postal_code")),
            Balance: reader.IsDBNull(reader.GetOrdinal("balance")) ? 0 : reader.GetDecimal(reader.GetOrdinal("balance")));
    }
}