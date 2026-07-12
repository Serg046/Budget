using Budget.Client.Models;

namespace Budget.Client.Repositories;

public interface ITransactionRepository
{
    Task Save(IReadOnlyList<TransactionDocument> transactions);

    Task<List<TransactionDocument>> Get(DateOnly from, DateOnly to);

    Task<List<string>> GetDistinctMerchantNames();

    Task<Dictionary<string, decimal>> GetTotalSpentByMerchant();

    Task<List<MonthlySpend>> GetMonthlySpendByMerchant(DateOnly from, DateOnly to);

    Task<DateOnly?> GetEarliestBookingDate();
}
