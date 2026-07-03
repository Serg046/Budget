using Budget.Models;

namespace Budget.Repositories;

public interface ISettingsRepository
{
    Task<SettingsDocument> Get();
}
