namespace Budget.Client.Repositories;

public interface ITrendMonthExclusionRepository
{
    Task<List<DateOnly>> GetAll();

    Task Add(DateOnly month);

    Task Remove(DateOnly month);
}
