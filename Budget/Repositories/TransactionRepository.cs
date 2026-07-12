using System.Globalization;
using Budget.Client.Models;
using Budget.Client.Repositories;
using Budget.Client.Services;
using MongoDB.Driver;

namespace Budget.Repositories;

public class TransactionRepository(IMongoDatabase database) : ITransactionRepository
{
    private readonly IMongoCollection<TransactionDocument> _collection = database.GetCollection<TransactionDocument>("transactions");

    public async Task Save(IReadOnlyList<TransactionDocument> transactions)
    {
        if (transactions.Count == 0)
        {
            return;
        }

        await _collection.InsertManyAsync(transactions);
    }

    public async Task<List<TransactionDocument>> Get(DateOnly from, DateOnly to)
    {
        var filter = Builders<TransactionDocument>.Filter.Gte(t => t.BookingDate, from) &
                     Builders<TransactionDocument>.Filter.Lte(t => t.BookingDate, to);

        var transactions = await _collection.Find(filter).ToListAsync();

        return transactions
            .OrderBy(t => t.EntryReference is null ? 0 : 1)
            .ThenByDescending(t => t.BookingDate)
            .ToList();
    }

    public async Task<List<string>> GetDistinctMerchantNames()
    {
        var filter = Builders<TransactionDocument>.Filter.Ne(t => t.Creditor!.Name, null);
        var rawNames = await _collection.Distinct(t => t.Creditor!.Name, filter).ToListAsync();

        return rawNames
            .Where(name => name is not null)
            .Select(name => MerchantNameNormalizer.Normalize(name!))
            .Distinct()
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();
    }

    public async Task<Dictionary<string, decimal>> GetTotalSpentByMerchant()
    {
        var filter = Builders<TransactionDocument>.Filter.Eq(t => t.CreditDebitIndicator, "DBIT") &
                     Builders<TransactionDocument>.Filter.Ne(t => t.Creditor!.Name, null);

        var spends = await _collection.Find(filter)
            .Project(t => new { Name = t.Creditor!.Name!, t.TransactionAmount.Value })
            .ToListAsync();

        var totals = new Dictionary<string, decimal>();
        foreach (var spend in spends)
        {
            var normalized = MerchantNameNormalizer.Normalize(spend.Name);
            var amount = decimal.Parse(spend.Value, CultureInfo.InvariantCulture);
            totals[normalized] = totals.GetValueOrDefault(normalized) + amount;
        }

        return totals;
    }

    public async Task<HashSet<string>> GetExistingEntryReferences(IEnumerable<string> entryReferences)
    {
        var filter = Builders<TransactionDocument>.Filter.In(t => t.EntryReference, entryReferences);

        var existing = await _collection.Find(filter)
            .Project(t => t.EntryReference)
            .ToListAsync();

        return existing.ToHashSet()!;
    }

    public async Task DeleteWithoutEntryReference()
    {
        await _collection.DeleteManyAsync(Builders<TransactionDocument>.Filter.Eq(t => t.EntryReference, null));
    }
}
