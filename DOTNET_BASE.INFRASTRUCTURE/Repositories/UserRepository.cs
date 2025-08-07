using System.Data;
using Dapper;
using DOTNET_BASE.CORE.Entities;
using DOTNET_BASE.CORE.Interfaces;

namespace DOTNET_BASE.INFRASTRUCTURE.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _dbConnection;

    public UserRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        const string query = "SELECT * FROM users WHERE id = @Id";
        return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Id = id });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        const string query = "SELECT * FROM users";
        return await _dbConnection.QueryAsync<User>(query);
    }

    public async Task<long> InsertAsync(User entity)
    {
        const string query = @"INSERT INTO users (username, email, fullname, createdat, updatedat) 
                              VALUES (@Username, @Email, @FullName, @CreatedAt, @UpdatedAt) 
                              RETURNING id";
        return await _dbConnection.ExecuteScalarAsync<long>(query, entity);
    }

    public async Task<bool> UpdateAsync(User entity)
    {
        const string query = @"UPDATE users 
                              SET username = @Username, email = @Email, fullname = @FullName, updatedat = @UpdatedAt 
                              WHERE id = @Id";
        var rowsAffected = await _dbConnection.ExecuteAsync(query, entity);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        const string query = "DELETE FROM users WHERE id = @Id";
        var rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string query = "SELECT * FROM users WHERE email = @Email";
        return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Email = email });
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        const string query = "SELECT * FROM users WHERE username = @Username";
        return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });
    }
}