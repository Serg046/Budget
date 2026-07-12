using System.Net.Http.Json;

namespace Budget.Client.Repositories;

public class MerchantNameExclusionApiClient(HttpClient http) : IMerchantNameExclusionRepository
{
    public async Task<List<string>> GetAll()
    {
        return await http.GetFromJsonAsync<List<string>>("api/merchant-name-exclusions") ?? [];
    }

    public async Task Add(string word)
    {
        var response = await http.PostAsJsonAsync("api/merchant-name-exclusions", word);
        response.EnsureSuccessStatusCode();
    }

    public async Task Remove(string word)
    {
        var response = await http.PostAsJsonAsync("api/merchant-name-exclusions/remove", word);
        response.EnsureSuccessStatusCode();
    }
}
