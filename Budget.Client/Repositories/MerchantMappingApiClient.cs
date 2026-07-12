using System.Net.Http.Json;
using Budget.Client.Models;

namespace Budget.Client.Repositories;

public class MerchantMappingApiClient(HttpClient http) : IMerchantMappingRepository
{
    public async Task<List<MerchantMapping>> GetAll()
    {
        return await http.GetFromJsonAsync<List<MerchantMapping>>("api/merchant-mappings") ?? [];
    }

    public async Task SetMapping(string mappedFrom, string mappedTo)
    {
        var response = await http.PostAsJsonAsync("api/merchant-mappings", new MerchantMapping { MappedFrom = mappedFrom, MappedTo = mappedTo });
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadFromJsonAsync<string>() ?? "Failed to save mapping.";
            throw new InvalidOperationException(message);
        }
    }

    public async Task RemoveMapping(string mappedFrom)
    {
        var response = await http.PostAsJsonAsync("api/merchant-mappings/remove", mappedFrom);
        response.EnsureSuccessStatusCode();
    }
}
