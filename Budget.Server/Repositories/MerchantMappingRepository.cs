using Budget.Client.Models;
using Budget.Client.Repositories;
using MongoDB.Driver;

namespace Budget.Server.Repositories;

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
            throw new ArgumentException($"'{mappedTo}' is not a known merchant name.");
        }

        var existingMappings = await GetAll();
        var chainTarget = existingMappings.FirstOrDefault(m => m.MappedFrom == mappedTo);
        if (chainTarget is not null)
        {
            throw new ArgumentException(
                $"'{mappedTo}' is itself mapped to '{chainTarget.MappedTo}'. Map to '{chainTarget.MappedTo}' directly instead.");
        }

        var filter = Builders<MerchantMapping>.Filter.Eq(m => m.MappedFrom, mappedFrom);
        var update = Builders<MerchantMapping>.Update
            .Set(m => m.MappedFrom, mappedFrom)
            .Set(m => m.MappedTo, mappedTo);

        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task RemoveMapping(string mappedFrom)
    {
        await _collection.DeleteOneAsync(Builders<MerchantMapping>.Filter.Eq(m => m.MappedFrom, mappedFrom));
    }
}
