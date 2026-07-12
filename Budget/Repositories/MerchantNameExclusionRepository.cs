using Budget.Client.Models;
using Budget.Client.Repositories;
using MongoDB.Driver;

namespace Budget.Repositories;

public class MerchantNameExclusionRepository(IMongoDatabase database) : IMerchantNameExclusionRepository
{
    private readonly IMongoCollection<MerchantNameExclusion> _collection =
        database.GetCollection<MerchantNameExclusion>("merchantNameExclusions");

    public async Task<List<string>> GetAll()
    {
        var exclusions = await _collection.Find(FilterDefinition<MerchantNameExclusion>.Empty).ToListAsync();
        return exclusions.Select(e => e.Word).ToList();
    }

    public async Task Add(string word)
    {
        var filter = Builders<MerchantNameExclusion>.Filter.Eq(e => e.Word, word);
        var update = Builders<MerchantNameExclusion>.Update.Set(e => e.Word, word);

        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task Remove(string word)
    {
        await _collection.DeleteOneAsync(Builders<MerchantNameExclusion>.Filter.Eq(e => e.Word, word));
    }
}
