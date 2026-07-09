namespace Budget.Client.Repositories;

public interface ISyncStatusRepository
{
    Task<string?> GetUsername();

    Task<bool> IsAdmin();

    Task<DateTime?> GetLastDataUpdate();

    Task<DateTime?> Sync();

    Task RefreshToken();
}
