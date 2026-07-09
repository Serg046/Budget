using Budget.Client.Repositories;
using Budget.Services;

namespace Budget.Repositories;

public class SyncStatusRepository(ISettingsRepository settingsRepository, SyncService syncService, IHttpContextAccessor httpContextAccessor) : ISyncStatusRepository
{
    public Task<string?> GetUsername() =>
        Task.FromResult(httpContextAccessor.HttpContext?.User.Identity?.Name);

    public Task<bool> IsAdmin() =>
        Task.FromResult(httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false);

    public async Task<DateTime?> GetLastDataUpdate()
    {
        var settings = await settingsRepository.Get();
        return settings.BankSession.LastDataUpdate;
    }

    public async Task<DateTime?> Sync()
    {
        await syncService.Sync();
        return await GetLastDataUpdate();
    }

    public async Task<string?> RefreshToken(string? code = null) => await syncService.RefreshToken(code);
}
