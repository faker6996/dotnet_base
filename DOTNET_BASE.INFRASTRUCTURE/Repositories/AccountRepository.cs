using System.Data;
using Dapper;
using DOTNET_BASE.CORE.Entities;
using DOTNET_BASE.CORE.Interfaces;

namespace DOTNET_BASE.INFRASTRUCTURE.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IDbConnection _dbConnection;

    public AccountRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<Account?> GetByIdAsync(long id)
    {
        const string query = "SELECT * FROM accounts WHERE id = @Id";
        return await _dbConnection.QueryFirstOrDefaultAsync<Account>(query, new { Id = id });
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        const string query = "SELECT * FROM accounts";
        return await _dbConnection.QueryAsync<Account>(query);
    }

    public async Task<long> InsertAsync(Account entity)
    {
        const string query = @"INSERT INTO accounts (accountname, accounttype, balance, isactive, createdat, updatedat) 
                              VALUES (@AccountName, @AccountType, @Balance, @IsActive, @CreatedAt, @UpdatedAt) 
                              RETURNING id";
        return await _dbConnection.ExecuteScalarAsync<long>(query, entity);
    }

    public async Task<bool> UpdateAsync(Account entity)
    {
        const string query = @"UPDATE accounts 
                              SET accountname = @AccountName, accounttype = @AccountType, balance = @Balance, 
                                  isactive = @IsActive, updatedat = @UpdatedAt 
                              WHERE id = @Id";
        var rowsAffected = await _dbConnection.ExecuteAsync(query, entity);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        const string query = "DELETE FROM accounts WHERE id = @Id";
        var rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = id });
        return rowsAffected > 0;
    }

    public async Task<IEnumerable<Account>> GetByTypeAsync(string accountType)
    {
        const string query = "SELECT * FROM accounts WHERE accounttype = @AccountType";
        return await _dbConnection.QueryAsync<Account>(query, new { AccountType = accountType });
    }

    public async Task<IEnumerable<Account>> GetActiveAccountsAsync()
    {
        const string query = "SELECT * FROM accounts WHERE isactive = true";
        return await _dbConnection.QueryAsync<Account>(query);
    }
}