using System.Net.Http.Json;
using Budget.Client.Models;

namespace Budget.Client.Repositories;

public class MerchantGroupApiClient(HttpClient http) : IMerchantGroupRepository
{
    public async Task<List<string>> GetGroups()
    {
        return await http.GetFromJsonAsync<List<string>>("api/merchant-groups") ?? [];
    }

    public async Task AddGroup(string name)
    {
        var response = await http.PostAsJsonAsync("api/merchant-groups", name);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveGroup(string name)
    {
        var response = await http.PostAsJsonAsync("api/merchant-groups/remove", name);
        response.EnsureSuccessStatusCode();
    }

    public async Task RenameGroup(string oldName, string newName)
    {
        var response = await http.PostAsJsonAsync(
            "api/merchant-groups/rename",
            new RenameGroupRequest { OldName = oldName, NewName = newName });

        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadFromJsonAsync<string>() ?? "Failed to rename group.";
            throw new InvalidOperationException(message);
        }
    }

    public async Task<List<MerchantGroupAssignment>> GetAssignments()
    {
        return await http.GetFromJsonAsync<List<MerchantGroupAssignment>>("api/merchant-group-assignments") ?? [];
    }

    public async Task SetAssignment(string merchantName, string groupName)
    {
        var response = await http.PostAsJsonAsync(
            "api/merchant-group-assignments",
            new MerchantGroupAssignment { MerchantName = merchantName, GroupName = groupName });
        response.EnsureSuccessStatusCode();
    }
}
