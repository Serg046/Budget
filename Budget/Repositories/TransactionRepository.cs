using Budget.Client.Models;
using Budget.Client.Repositories;
using MongoDB.Driver;

namespace Budget.Repositories;

public class TransactionRepository(IMongoDatabase database) : ITransactionRepository
{
    private readonly IMongoCollection<TransactionDocument> _collection = database.GetCollection<TransactionDocument>("transactions");

    public async Task Save(IEnumerable<TransactionDocument> transactions)
    {
        var list = transactions.ToList();
        if (list.Count == 0)
        {
            return;
        }

        await _collection.InsertManyAsync(list);
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
