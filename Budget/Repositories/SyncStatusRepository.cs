using Budget.Client.Repositories;

namespace Budget.Repositories;

public class SyncStatusRepository(ISettingsRepository settingsRepository, IConfiguration configuration) : ISyncStatusRepository
{
    public async Task<DateTime?> GetLastDataUpdate()
    {
        var settings = await settingsRepository.Get();
        return settings.BankSession.LastDataUpdate;
    }

    public Task<bool> ValidatePassword(string password)
    {
        var expected = configuration["Sync:Password"];
        return Task.FromResult(!string.IsNullOrEmpty(expected) && password == expected);
    }
}
