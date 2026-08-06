using Budget.Client.Models;
using Budget.Client.Repositories;
using MongoDB.Driver;

namespace Budget.Server.Repositories;

public class TrendMonthExclusionRepository(IMongoDatabase database) : ITrendMonthExclusionRepository
{
    private readonly IMongoCollection<TrendMonthExclusion> _collection =
        database.GetCollection<TrendMonthExclusion>("trendMonthExclusions");

    public async Task<List<DateOnly>> GetAll()
    {
        var exclusions = await _collection.Find(FilterDefinition<TrendMonthExclusion>.Empty).ToListAsync();
        return exclusions.Select(e => e.Month).ToList();
    }

    public async Task Add(DateOnly month)
    {
        var filter = Builders<TrendMonthExclusion>.Filter.Eq(e => e.Month, month);
        var update = Builders<TrendMonthExclusion>.Update.Set(e => e.Month, month);

        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task Remove(DateOnly month)
    {
        await _collection.DeleteOneAsync(Builders<TrendMonthExclusion>.Filter.Eq(e => e.Month, month));
    }
}
