using DOTNET_BASE.CORE.Interfaces;
using AccountEntity = DOTNET_BASE.CORE.Entities.Account;

namespace DOTNET_BASE.APPLICATION.Account;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccountsAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Select(MapToDto);
    }

    public async Task<AccountDto?> GetAccountByIdAsync(long id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        return account == null ? null : MapToDto(account);
    }

    public async Task<IEnumerable<AccountDto>> GetAccountsByTypeAsync(string accountType)
    {
        var accounts = await _accountRepository.GetByTypeAsync(accountType);
        return accounts.Select(MapToDto);
    }

    public async Task<IEnumerable<AccountDto>> GetActiveAccountsAsync()
    {
        var accounts = await _accountRepository.GetActiveAccountsAsync();
        return accounts.Select(MapToDto);
    }

    public async Task<AccountDto> CreateAccountAsync(CreateAccountDto createAccountDto)
    {
        var account = new AccountEntity
        {
            AccountName = createAccountDto.AccountName,
            AccountType = createAccountDto.AccountType,
            Balance = createAccountDto.Balance,
            IsActive = createAccountDto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var newId = await _accountRepository.InsertAsync(account);
        account.Id = newId;
        return MapToDto(account);
    }

    public async Task<AccountDto> UpdateAccountAsync(long id, UpdateAccountDto updateAccountDto)
    {
        var existingAccount = await _accountRepository.GetByIdAsync(id);
        if (existingAccount == null)
            throw new ArgumentException($"Account with ID {id} not found");

        existingAccount.AccountName = updateAccountDto.AccountName;
        existingAccount.AccountType = updateAccountDto.AccountType;
        existingAccount.Balance = updateAccountDto.Balance;
        existingAccount.IsActive = updateAccountDto.IsActive;
        existingAccount.UpdatedAt = DateTime.UtcNow;

        await _accountRepository.UpdateAsync(existingAccount);
        return MapToDto(existingAccount);
    }

    public async Task<bool> DeleteAccountAsync(long id)
    {
        return await _accountRepository.DeleteAsync(id);
    }

    private static AccountDto MapToDto(AccountEntity account)
    {
        return new AccountDto
        {
            Id = account.Id,
            AccountName = account.AccountName,
            AccountType = account.AccountType,
            Balance = account.Balance,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };
    }
}