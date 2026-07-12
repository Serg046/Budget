using Budget.Client.Models;

namespace Budget.Client.Repositories;

public interface IMerchantMappingRepository
{
    Task<List<MerchantMapping>> GetAll();

    Task SetMapping(string mappedFrom, string mappedTo);

    Task RemoveMapping(string mappedFrom);
}
