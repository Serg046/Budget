using Budget.Client.Models;
using Budget.Client.Repositories;
using MongoDB.Driver;

namespace Budget.Server.Repositories;

public class MerchantGroupRepository(IMongoDatabase database) : IMerchantGroupRepository
{
    private readonly IMongoCollection<MerchantGroup> _groups = database.GetCollection<MerchantGroup>("merchantGroups");
    private readonly IMongoCollection<MerchantGroupAssignment> _assignments =
        database.GetCollection<MerchantGroupAssignment>("merchantGroupAssignments");

    public async Task<List<string>> GetGroups()
    {
        var groups = await _groups.Find(FilterDefinition<MerchantGroup>.Empty).ToListAsync();
        return groups.Select(g => g.Name).ToList();
    }

    public async Task AddGroup(string name)
    {
        if (name == IMerchantGroupRepository.OthersGroupName)
        {
            throw new ArgumentException($"'{IMerchantGroupRepository.OthersGroupName}' is a reserved group name.");
        }

        var filter = Builders<MerchantGroup>.Filter.Eq(g => g.Name, name);
        var update = Builders<MerchantGroup>.Update.Set(g => g.Name, name);

        await _groups.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task RemoveGroup(string name)
    {
        await _groups.DeleteOneAsync(Builders<MerchantGroup>.Filter.Eq(g => g.Name, name));
        await _assignments.DeleteManyAsync(Builders<MerchantGroupAssignment>.Filter.Eq(a => a.GroupName, name));
    }

    public async Task RenameGroup(string oldName, string newName)
    {
        if (newName == IMerchantGroupRepository.OthersGroupName)
        {
            throw new ArgumentException($"'{IMerchantGroupRepository.OthersGroupName}' is a reserved group name.");
        }

        var duplicate = await _groups.Find(Builders<MerchantGroup>.Filter.Eq(g => g.Name, newName)).AnyAsync();
        if (duplicate)
        {
            throw new ArgumentException($"A group named '{newName}' already exists.");
        }

        await _groups.UpdateOneAsync(
            Builders<MerchantGroup>.Filter.Eq(g => g.Name, oldName),
            Builders<MerchantGroup>.Update.Set(g => g.Name, newName));

        await _assignments.UpdateManyAsync(
            Builders<MerchantGroupAssignment>.Filter.Eq(a => a.GroupName, oldName),
            Builders<MerchantGroupAssignment>.Update.Set(a => a.GroupName, newName));
    }

    public async Task<List<MerchantGroupAssignment>> GetAssignments()
    {
        return await _assignments.Find(FilterDefinition<MerchantGroupAssignment>.Empty).ToListAsync();
    }

    public async Task SetAssignment(string merchantName, string groupName)
    {
        var filter = Builders<MerchantGroupAssignment>.Filter.Eq(a => a.MerchantName, merchantName);

        if (groupName == IMerchantGroupRepository.OthersGroupName)
        {
            await _assignments.DeleteOneAsync(filter);
            return;
        }

        var update = Builders<MerchantGroupAssignment>.Update
            .Set(a => a.MerchantName, merchantName)
            .Set(a => a.GroupName, groupName);

        await _assignments.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }
}
