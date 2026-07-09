namespace Budget.Client.Services;

public class SyncStatusState
{
    public event Action? Changed;
    public event Action? SyncCompleted;

    private bool _isSyncing;

    public bool IsSyncing
    {
        get => _isSyncing;
        set
        {
            _isSyncing = value;
            Changed?.Invoke();
        }
    }

    public void NotifySyncCompleted() => SyncCompleted?.Invoke();
}
