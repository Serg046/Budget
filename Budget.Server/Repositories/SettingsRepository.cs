using Budget.Server.Models;
using MongoDB.Driver;

namespace Budget.Server.Repositories;

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

    public async Task UpdateLastDataUpdate(DateTime lastDataUpdate)
    {
        var update = Builders<SettingsDocument>.Update
            .Set(s => s.BankSession.LastDataUpdate, lastDataUpdate);

        await _collection.UpdateOneAsync(FilterDefinition<SettingsDocument>.Empty, update);
    }

    public async Task UpdateLastTokenUpdate(DateTime lastTokenUpdate)
    {
        var update = Builders<SettingsDocument>.Update
            .Set(s => s.BankSession.LastTokenUpdate, lastTokenUpdate);

        await _collection.UpdateOneAsync(FilterDefinition<SettingsDocument>.Empty, update);
    }

    public async Task UpdateAccount(string accountId, DateTime lastTokenUpdate)
    {
        var update = Builders<SettingsDocument>.Update
            .Set(s => s.BankSession.AccountId, accountId)
            .Set(s => s.BankSession.LastTokenUpdate, lastTokenUpdate);

        await _collection.UpdateOneAsync(FilterDefinition<SettingsDocument>.Empty, update);
    }
}
