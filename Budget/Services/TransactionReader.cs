using System.Text.Json;
using Budget.Client.Models;

namespace Budget.Services;

public class TransactionReader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly HttpClient _http;

    public TransactionReader(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<TransactionDocument>> Get(string accountUid)
    {
        var transactions = new List<TransactionDocument>();
        string? continuationKey = null;

        do
        {
            var url = continuationKey is null
                ? $"accounts/{accountUid}/transactions"
                : $"accounts/{accountUid}/transactions?continuation_key={Uri.EscapeDataString(continuationKey)}";

            var resp = await _http.GetAsync(url);
            var body = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception($"Request failed ({resp.StatusCode}): {body}");
            }

            var page = JsonSerializer.Deserialize<TransactionsPage>(body, JsonOptions)!;
            transactions.AddRange(page.Transactions);
            continuationKey = page.ContinuationKey;
        } while (continuationKey is not null);

        return transactions;
    }

    private class TransactionsPage
    {
        public List<TransactionDocument> Transactions { get; set; } = [];
        public string? ContinuationKey { get; set; }
    }
}
