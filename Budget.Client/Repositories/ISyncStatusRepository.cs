namespace Budget.Client.Repositories;

public interface ISyncStatusRepository
{
    Task<string?> GetUsername();

    Task<DateTime?> GetLastDataUpdate();

    Task<bool> ValidatePassword(string password);

    Task<DateTime?> Sync(string password);
}
