using SpendPulse.Server.Models;

namespace SpendPulse.Server.Repositories;

public interface ISettingsRepository
{
    Task<SettingsDocument> Get();

    Task UpdateLastDataUpdate(DateTime lastDataUpdate);

    Task UpdateLastTokenUpdate(DateTime lastTokenUpdate);

    Task UpdateAccount(string account, DateTime lastTokenUpdate);
}
