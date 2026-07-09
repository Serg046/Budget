using Budget.Models;

namespace Budget.Repositories;

public interface ISettingsRepository
{
    Task<SettingsDocument> Get();

    Task UpdateLastDataUpdate(DateTime lastDataUpdate);

    Task UpdateLastTokenUpdate(DateTime lastTokenUpdate);
}
