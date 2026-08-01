using Finance.Application.DTOs;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeneralLedgerController : ControllerBase
{
    private readonly IGeneralLedgerService _glService;

    public GeneralLedgerController(IGeneralLedgerService glService)
    {
        _glService = glService;
    }

    // Chart of Accounts
    [HttpGet("accounts")]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAccounts()
    {
        var accounts = await _glService.GetAllAccountsAsync();
        return Ok(accounts);
    }

    [HttpGet("accounts/{id}")]
    public async Task<ActionResult<AccountDto>> GetAccountById(int id)
    {
        var account = await _glService.GetAccountByIdAsync(id);
        if (account == null)
            return NotFound();

        return Ok(account);
    }

    [HttpGet("accounts/by-type/{type}")]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccountsByType(AccountType type)
    {
        var accounts = await _glService.GetAccountsByTypeAsync(type);
        return Ok(accounts);
    }

    [HttpPost("accounts")]
    public async Task<ActionResult<AccountDto>> CreateAccount(CreateAccountRequest request)
    {
        var account = await _glService.CreateAccountAsync(request);
        return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, account);
    }

    [HttpPut("accounts/{id}")]
    public async Task<ActionResult<AccountDto>> UpdateAccount(int id, UpdateAccountRequest request)
    {
        try
        {
            var account = await _glService.UpdateAccountAsync(id, request);
            return Ok(account);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("accounts/{id}")]
    public async Task<ActionResult> DeleteAccount(int id)
    {
        try
        {
            await _glService.DeleteAccountAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Journal Entries
    [HttpGet("journal-entries")]
    public async Task<ActionResult<IEnumerable<JournalEntryDto>>> GetAllJournalEntries()
    {
        var entries = await _glService.GetAllJournalEntriesAsync();
        return Ok(entries);
    }

    [HttpGet("journal-entries/{id}")]
    public async Task<ActionResult<JournalEntryDto>> GetJournalEntryById(int id)
    {
        var entry = await _glService.GetJournalEntryByIdAsync(id);
        if (entry == null)
            return NotFound();

        return Ok(entry);
    }

    [HttpPost("journal-entries")]
    public async Task<ActionResult<JournalEntryDto>> CreateJournalEntry(CreateJournalEntryRequest request)
    {
        try
        {
            var entry = await _glService.CreateJournalEntryAsync(request);
            return CreatedAtAction(nameof(GetJournalEntryById), new { id = entry.Id }, entry);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("journal-entries/{id}/post")]
    public async Task<ActionResult> PostJournalEntry(int id)
    {
        try
        {
            await _glService.PostJournalEntryAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("journal-entries/{id}/void")]
    public async Task<ActionResult> VoidJournalEntry(int id)
    {
        try
        {
            await _glService.VoidJournalEntryAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Trial Balance
    [HttpGet("trial-balance")]
    public async Task<ActionResult<TrialBalanceDto>> GetTrialBalance([FromQuery] DateTime? asOfDate = null)
    {
        var date = asOfDate ?? DateTime.Now;
        var trialBalance = await _glService.GetTrialBalanceAsync(date);
        return Ok(trialBalance);
    }

    // Account History
    [HttpGet("accounts/{accountId}/history")]
    public async Task<ActionResult<AccountHistoryDto>> GetAccountHistory(
        int accountId, 
        [FromQuery] DateTime? fromDate = null, 
        [FromQuery] DateTime? toDate = null)
    {
        var from = fromDate ?? DateTime.Now.AddMonths(-1);
        var to = toDate ?? DateTime.Now;

        try
        {
            var history = await _glService.GetAccountHistoryAsync(accountId, from, to);
            return Ok(history);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
