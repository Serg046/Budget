using Budget.Client.Models;
using Budget.Client.Repositories;
using MongoDB.Driver;

namespace Budget.Repositories;

public class MerchantMappingRepository(IMongoDatabase database, ITransactionRepository transactionRepository) : IMerchantMappingRepository
{
    private readonly IMongoCollection<MerchantMapping> _collection = database.GetCollection<MerchantMapping>("merchantMappings");

    public async Task<List<MerchantMapping>> GetAll()
    {
        return await _collection.Find(FilterDefinition<MerchantMapping>.Empty).ToListAsync();
    }

    public async Task SetMapping(string mappedFrom, string mappedTo)
    {
        var knownMerchantNames = await transactionRepository.GetDistinctMerchantNames();
        if (!knownMerchantNames.Contains(mappedTo))
        {
            throw new ArgumentException($"'{mappedTo}' is not a known merchant name.", nameof(mappedTo));
        }

        var filter = Builders<MerchantMapping>.Filter.Eq(m => m.MappedFrom, mappedFrom);
        var update = Builders<MerchantMapping>.Update
            .Set(m => m.MappedFrom, mappedFrom)
            .Set(m => m.MappedTo, mappedTo);

        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }
}
