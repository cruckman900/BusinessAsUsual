using Finance.Application.DTOs;

namespace Finance.Application.Services;

public interface IGeneralLedgerService
{
    // Chart of Accounts
    Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
    Task<AccountDto?> GetAccountByIdAsync(int id);
    Task<AccountDto> CreateAccountAsync(CreateAccountRequest request);
    Task<AccountDto> UpdateAccountAsync(int id, UpdateAccountRequest request);
    Task DeleteAccountAsync(int id);
    Task<IEnumerable<AccountDto>> GetAccountsByTypeAsync(AccountType type);

    // Journal Entries
    Task<IEnumerable<JournalEntryDto>> GetAllJournalEntriesAsync();
    Task<JournalEntryDto?> GetJournalEntryByIdAsync(int id);
    Task<JournalEntryDto> CreateJournalEntryAsync(CreateJournalEntryRequest request);
    Task PostJournalEntryAsync(int id);
    Task VoidJournalEntryAsync(int id);

    // Trial Balance
    Task<TrialBalanceDto> GetTrialBalanceAsync(DateTime asOfDate);

    // Account History
    Task<AccountHistoryDto> GetAccountHistoryAsync(int accountId, DateTime fromDate, DateTime toDate);
}
