using System.Net.Http.Json;

namespace Budget.Client.Repositories;

public class SyncStatusApiClient(HttpClient http) : ISyncStatusRepository
{
    public async Task<string?> GetUsername()
    {
        return await http.GetFromJsonAsync<string?>("api/sync-status/username");
    }

    public async Task<bool> IsAdmin()
    {
        return await http.GetFromJsonAsync<bool>("api/sync-status/is-admin");
    }

    public async Task<DateTime?> GetLastDataUpdate()
    {
        return await http.GetFromJsonAsync<DateTime?>("api/sync-status/last-data-update");
    }

    public async Task<DateTime?> Sync()
    {
        var response = await http.PostAsync("api/sync-status/sync", null);
        return await response.Content.ReadFromJsonAsync<DateTime?>();
    }

    public async Task RefreshToken()
    {
        await http.PostAsync("api/sync-status/refresh-token", null);
    }
}
