using Budget.Models;
using MongoDB.Driver;

namespace Budget.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly IMongoCollection<SettingsDocument> _collection;

    public SettingsRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<SettingsDocument>("settings");
    }

    public async Task<SettingsDocument> Get()
    {
        return await _collection.Find(FilterDefinition<SettingsDocument>.Empty).SingleAsync();
    }
}
