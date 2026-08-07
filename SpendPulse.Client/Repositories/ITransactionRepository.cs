using SpendPulse.Client.Models;

namespace SpendPulse.Client.Repositories;

public interface ITransactionRepository
{
    Task Save(IReadOnlyList<TransactionDocument> transactions);

    Task<List<TransactionDocument>> Get(DateOnly from, DateOnly to);

    Task<List<string>> GetDistinctMerchantNames();

    Task<Dictionary<string, decimal>> GetTotalSpentByMerchant();

    Task<List<MonthlySpend>> GetMonthlySpendByMerchant(DateOnly from, DateOnly to);

    Task<List<MonthlySpend>> GetTopMerchantsMonthlySpend(DateOnly from, DateOnly to, int topN);

    Task<List<MonthlySpend>> GetMonthlySpendForMerchant(DateOnly from, DateOnly to, string merchantName);

    Task<DateOnly?> GetEarliestBookingDate();
}
