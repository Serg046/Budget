using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Budget.Client.Models;
using Budget.Repositories;

namespace Budget.Services;

public class SyncService(IConfiguration configuration, ISettingsRepository settingsRepository, TransactionRepository transactionRepository)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public async Task Sync()
    {
        var settings = await settingsRepository.Get();

        var rsa = LoadPrivateKey(configuration["EnableBanking:PrivateKeyPath"]!);
        var jwt = BuildJwt(settings.BankSession.ApplicationId, rsa);

        using var http = new HttpClient { BaseAddress = new Uri("https://api.enablebanking.com/") };
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        await transactionRepository.DeleteWithoutEntryReference();

        string? continuationKey = null;
        do
        {
            var (transactions, nextContinuationKey) = await FetchPage(http, settings.BankSession.Account, continuationKey);

            var entryReferences = transactions
                .Where(t => t.EntryReference is not null)
                .Select(t => t.EntryReference!)
                .ToList();
            var existingEntryReferences = await transactionRepository.GetExistingEntryReferences(entryReferences);

            var newTransactions = transactions
                .Where(t => t.EntryReference is null || !existingEntryReferences.Contains(t.EntryReference))
                .ToList();
            await transactionRepository.Save(newTransactions);

            continuationKey = existingEntryReferences.Count == 0 ? nextContinuationKey : null;
        } while (continuationKey is not null);

        await settingsRepository.UpdateLastDataUpdate(DateTime.UtcNow);
    }

    public async Task RefreshToken()
    {
        // TODO: actually request a fresh token from the bank; for now just record that a refresh happened.
        await settingsRepository.UpdateLastTokenUpdate(DateTime.UtcNow);
    }

    private static async Task<(List<TransactionDocument> Transactions, string? ContinuationKey)> FetchPage(HttpClient http, string accountUid, string? continuationKey)
    {
        var url = continuationKey is null
            ? $"accounts/{accountUid}/transactions"
            : $"accounts/{accountUid}/transactions?continuation_key={Uri.EscapeDataString(continuationKey)}";

        var resp = await http.GetAsync(url);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
        {
            throw new Exception($"Request failed ({resp.StatusCode}): {body}");
        }

        var page = JsonSerializer.Deserialize<TransactionsPage>(body, JsonOptions)!;
        return (page.Transactions, page.ContinuationKey);
    }

    private static RSA LoadPrivateKey(string path)
    {
        var pem = File.ReadAllText(path);
        var key = RSA.Create();
        key.ImportFromPem(pem);
        return key;
    }

    private static string BuildJwt(string appId, RSA key)
    {
        var header = new { typ = "JWT", alg = "RS256", kid = appId };
        var now = DateTimeOffset.UtcNow;
        var body = new
        {
            iss = "enablebanking.com",
            aud = "api.enablebanking.com",
            iat = now.ToUnixTimeSeconds(),
            exp = now.AddHours(1).ToUnixTimeSeconds()
        };

        var headerJson = JsonSerializer.Serialize(header);
        var bodyJson = JsonSerializer.Serialize(body);
        var headerB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
        var bodyB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(bodyJson));
        var signingInput = $"{headerB64}.{bodyB64}";

        var signature = key.SignData(Encoding.UTF8.GetBytes(signingInput), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var signatureB64 = Base64UrlEncode(signature);

        return $"{signingInput}.{signatureB64}";
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private class TransactionsPage
    {
        public List<TransactionDocument> Transactions { get; set; } = [];
        public string? ContinuationKey { get; set; }
    }
}
