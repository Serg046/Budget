using System.Net.Http.Json;

namespace Budget.Client.Repositories;

public class SyncStatusApiClient(HttpClient http) : ISyncStatusRepository
{
    public async Task<DateTime?> GetLastDataUpdate()
    {
        return await http.GetFromJsonAsync<DateTime?>("api/sync-status/last-data-update");
    }

    public async Task<bool> ValidatePassword(string password)
    {
        var response = await http.PostAsJsonAsync("api/sync-status/validate", password);
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}
