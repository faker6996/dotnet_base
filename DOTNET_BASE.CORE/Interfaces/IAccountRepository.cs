using DOTNET_BASE.CORE.Entities;

namespace DOTNET_BASE.CORE.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(long id);
    Task<IEnumerable<Account>> GetAllAsync();
    Task<long> InsertAsync(Account entity);
    Task<bool> UpdateAsync(Account entity);
    Task<bool> DeleteAsync(long id);
    Task<IEnumerable<Account>> GetByTypeAsync(string accountType);
    Task<IEnumerable<Account>> GetActiveAccountsAsync();
}