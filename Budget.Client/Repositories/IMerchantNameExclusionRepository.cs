namespace Budget.Client.Repositories;

public interface IMerchantNameExclusionRepository
{
    Task<List<string>> GetAll();

    Task Add(string word);

    Task Remove(string word);
}
