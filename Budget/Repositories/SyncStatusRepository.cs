using Budget.Client.Repositories;
using Budget.Services;

namespace Budget.Repositories;

public class SyncStatusRepository(ISettingsRepository settingsRepository, IConfiguration configuration, SyncService syncService, IHttpContextAccessor httpContextAccessor) : ISyncStatusRepository
{
    public Task<string?> GetUsername() =>
        Task.FromResult(httpContextAccessor.HttpContext?.User.Identity?.Name);

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

    public async Task<DateTime?> Sync(string password)
    {
        var isDataSync = await ValidatePassword(password);
        var isTokenRefresh = IsTokenRefreshPassword(password);

        if (!isDataSync && !isTokenRefresh)
        {
            return null;
        }

        await syncService.Sync();

        if (isTokenRefresh)
        {
            await syncService.RefreshToken();
        }

        return await GetLastDataUpdate();
    }

    private bool IsTokenRefreshPassword(string password)
    {
        var expected = configuration["Sync:TokenRefreshPassword"];
        return !string.IsNullOrEmpty(expected) && password == expected;
    }
}
