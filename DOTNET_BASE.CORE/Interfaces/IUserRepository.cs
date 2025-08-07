using DOTNET_BASE.CORE.Entities;

namespace DOTNET_BASE.CORE.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<long> InsertAsync(User entity);
    Task<bool> UpdateAsync(User entity);
    Task<bool> DeleteAsync(long id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
}