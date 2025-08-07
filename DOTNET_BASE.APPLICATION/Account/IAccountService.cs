namespace DOTNET_BASE.APPLICATION.Account;

public interface IAccountService
{
    Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
    Task<AccountDto?> GetAccountByIdAsync(long id);
    Task<IEnumerable<AccountDto>> GetAccountsByTypeAsync(string accountType);
    Task<IEnumerable<AccountDto>> GetActiveAccountsAsync();
    Task<AccountDto> CreateAccountAsync(CreateAccountDto createAccountDto);
    Task<AccountDto> UpdateAccountAsync(long id, UpdateAccountDto updateAccountDto);
    Task<bool> DeleteAccountAsync(long id);
}