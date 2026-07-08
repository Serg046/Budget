using System.Net.Http.Json;
using Budget.Client.Models;

namespace Budget.Client.Repositories;

public class TransactionApiClient(HttpClient http) : ITransactionRepository
{
    // TODO: remove if not needed
    public Task Save(IEnumerable<TransactionDocument> transactions) =>
        throw new NotSupportedException();

    public async Task<List<TransactionDocument>> Get(DateOnly from, DateOnly to)
    {
        return await http.GetFromJsonAsync<List<TransactionDocument>>(
            $"api/transactions?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}") ?? [];
    }
}
