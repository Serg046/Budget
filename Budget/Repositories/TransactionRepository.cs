using Budget.Models;
using MongoDB.Driver;

namespace Budget.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IMongoCollection<TransactionDocument> _collection;

    public TransactionRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<TransactionDocument>("transactions");
    }

    public async Task Save(IEnumerable<TransactionDocument> transactions)
    {
        var list = transactions.ToList();
        if (list.Count == 0)
        {
            return;
        }

        await _collection.InsertManyAsync(list);
    }

    public async Task<List<TransactionDocument>> Get()
    {
        return await _collection.Find(FilterDefinition<TransactionDocument>.Empty).ToListAsync();
    }
}
