using Finance.Application.DTOs;
using Finance.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankingController : ControllerBase
{
    private readonly IBankingService _bankingService;

    public BankingController(IBankingService bankingService)
    {
        _bankingService = bankingService;
    }

    // Bank Accounts
    [HttpGet("accounts")]
    public async Task<ActionResult<IEnumerable<BankAccountDto>>> GetAllAccounts()
        => Ok(await _bankingService.GetAllBankAccountsAsync());

    [HttpGet("accounts/{id}")]
    public async Task<ActionResult<BankAccountDto>> GetAccountById(string id)
    {
        var account = await _bankingService.GetBankAccountByIdAsync(id);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpPost("accounts")]
    public async Task<ActionResult<BankAccountDto>> CreateAccount(CreateBankAccountRequest request)
    {
        var account = await _bankingService.CreateBankAccountAsync(request);
        return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, account);
    }

    [HttpPut("accounts/{id}")]
    public async Task<ActionResult<BankAccountDto>> UpdateAccount(string id, CreateBankAccountRequest request)
    {
        try
        {
            return Ok(await _bankingService.UpdateBankAccountAsync(id, request));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("accounts/{id}")]
    public async Task<ActionResult> DeleteAccount(string id)
    {
        await _bankingService.DeleteBankAccountAsync(id);
        return NoContent();
    }

    // Transactions
    [HttpGet("accounts/{accountId}/transactions")]
    public async Task<ActionResult<IEnumerable<BankTransactionDto>>> GetAccountTransactions(string accountId)
        => Ok(await _bankingService.GetTransactionsByAccountAsync(accountId));

    [HttpGet("transactions/{id}")]
    public async Task<ActionResult<BankTransactionDto>> GetTransactionById(string id)
    {
        var transaction = await _bankingService.GetTransactionByIdAsync(id);
        return transaction is null ? NotFound() : Ok(transaction);
    }

    [HttpPost("transactions")]
    public async Task<ActionResult<BankTransactionDto>> CreateTransaction(CreateBankTransactionRequest request)
    {
        try
        {
            var transaction = await _bankingService.CreateTransactionAsync(request);
            return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("transactions/{id}")]
    public async Task<ActionResult> DeleteTransaction(string id)
    {
        await _bankingService.DeleteTransactionAsync(id);
        return NoContent();
    }

    // Reconciliation
    [HttpPost("reconcile")]
    public async Task<ActionResult<BankTransactionDto>> ReconcileTransaction(ReconcileTransactionRequest request)
    {
        try
        {
            return Ok(await _bankingService.ReconcileTransactionAsync(request));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("accounts/{accountId}/unreconciled")]
    public async Task<ActionResult<IEnumerable<BankTransactionDto>>> GetUnreconciledTransactions(string accountId)
        => Ok(await _bankingService.GetUnreconciledTransactionsAsync(accountId));
}
