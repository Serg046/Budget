namespace Budget.Client.Repositories;

public interface ISyncStatusRepository
{
    Task<DateTime?> GetLastDataUpdate();

    Task<bool> ValidatePassword(string password);

    Task<DateTime?> Sync(string password);
}
