using Budget.Client.Models;

namespace Budget.Client.Repositories;

public interface ITransactionRepository
{
    Task Save(IReadOnlyList<TransactionDocument> transactions);

    Task<List<TransactionDocument>> Get(DateOnly from, DateOnly to);
}
