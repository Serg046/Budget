using Budget.Server.Models;

namespace Budget.Server.Repositories;

public interface ISettingsRepository
{
    Task<SettingsDocument> Get();

    Task UpdateLastDataUpdate(DateTime lastDataUpdate);

    Task UpdateLastTokenUpdate(DateTime lastTokenUpdate);

    Task UpdateAccount(string account, DateTime lastTokenUpdate);
}
