using System.Net.Http.Json;
using Budget.Client.Models;

namespace Budget.Client.Repositories;

public class TransactionApiClient(HttpClient http) : ITransactionRepository
{
    // TODO: remove if not needed
    public Task Save(IReadOnlyList<TransactionDocument> transactions) =>
        throw new NotSupportedException();

    public async Task<List<TransactionDocument>> Get(DateOnly from, DateOnly to)
    {
        return await http.GetFromJsonAsync<List<TransactionDocument>>(
            $"api/transactions?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}") ?? [];
    }

    public async Task<List<string>> GetDistinctMerchantNames()
    {
        return await http.GetFromJsonAsync<List<string>>("api/merchants") ?? [];
    }

    public async Task<Dictionary<string, decimal>> GetTotalSpentByMerchant()
    {
        return await http.GetFromJsonAsync<Dictionary<string, decimal>>("api/merchant-totals") ?? [];
    }

    public async Task<List<MonthlySpend>> GetMonthlySpendByMerchant(DateOnly from, DateOnly to)
    {
        return await http.GetFromJsonAsync<List<MonthlySpend>>(
            $"api/monthly-spend?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}") ?? [];
    }

    public async Task<DateOnly?> GetEarliestBookingDate()
    {
        return await http.GetFromJsonAsync<DateOnly?>("api/earliest-transaction-date");
    }
}
