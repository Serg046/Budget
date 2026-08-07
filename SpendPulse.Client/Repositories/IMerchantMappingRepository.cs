using SpendPulse.Client.Models;

namespace SpendPulse.Client.Repositories;

public interface IMerchantMappingRepository
{
    Task<List<MerchantMapping>> GetAll();

    Task SetMapping(string mappedFrom, string mappedTo);

    Task RemoveMapping(string mappedFrom);
}
