using System.Net.Http.Json;

namespace Budget.Client.Repositories;

public class TrendMonthExclusionApiClient(HttpClient http) : ITrendMonthExclusionRepository
{
    public async Task<List<DateOnly>> GetAll()
    {
        return await http.GetFromJsonAsync<List<DateOnly>>("api/trend-month-exclusions") ?? [];
    }

    public async Task Add(DateOnly month)
    {
        var response = await http.PostAsJsonAsync("api/trend-month-exclusions", month);
        response.EnsureSuccessStatusCode();
    }

    public async Task Remove(DateOnly month)
    {
        var response = await http.PostAsJsonAsync("api/trend-month-exclusions/remove", month);
        response.EnsureSuccessStatusCode();
    }
}
