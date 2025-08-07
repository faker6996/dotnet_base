using Microsoft.AspNetCore.Mvc;
using DOTNET_BASE.APPLICATION.Account;

namespace DOTNET_BASE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetAccount(long id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);
        if (account == null)
            return NotFound();

        return Ok(account);
    }

    [HttpGet("by-type/{accountType}")]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccountsByType(string accountType)
    {
        var accounts = await _accountService.GetAccountsByTypeAsync(accountType);
        return Ok(accounts);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetActiveAccounts()
    {
        var accounts = await _accountService.GetActiveAccountsAsync();
        return Ok(accounts);
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> CreateAccount(CreateAccountDto createAccountDto)
    {
        try
        {
            var account = await _accountService.CreateAccountAsync(createAccountDto);
            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AccountDto>> UpdateAccount(long id, UpdateAccountDto updateAccountDto)
    {
        try
        {
            var account = await _accountService.UpdateAccountAsync(id, updateAccountDto);
            return Ok(account);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAccount(long id)
    {
        var result = await _accountService.DeleteAccountAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}