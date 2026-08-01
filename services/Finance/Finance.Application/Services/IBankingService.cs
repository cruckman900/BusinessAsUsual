using Finance.Application.DTOs;

namespace Finance.Application.Services;

public interface IBankingService
{
    // Bank Accounts
    Task<IEnumerable<BankAccountDto>> GetAllBankAccountsAsync();
    Task<BankAccountDto?> GetBankAccountByIdAsync(string id);
    Task<BankAccountDto> CreateBankAccountAsync(CreateBankAccountRequest request);
    Task<BankAccountDto> UpdateBankAccountAsync(string id, CreateBankAccountRequest request);
    Task DeleteBankAccountAsync(string id);

    // Transactions
    Task<IEnumerable<BankTransactionDto>> GetTransactionsByAccountAsync(string accountId);
    Task<BankTransactionDto?> GetTransactionByIdAsync(string id);
    Task<BankTransactionDto> CreateTransactionAsync(CreateBankTransactionRequest request);
    Task DeleteTransactionAsync(string id);

    // Reconciliation
    Task<BankTransactionDto> ReconcileTransactionAsync(ReconcileTransactionRequest request);
    Task<IEnumerable<BankTransactionDto>> GetUnreconciledTransactionsAsync(string accountId);
}
