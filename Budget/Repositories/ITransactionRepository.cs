using Budget.Models;

namespace Budget.Repositories;

public interface ITransactionRepository
{
    Task Save(IEnumerable<TransactionDocument> transactions);

    Task<List<TransactionDocument>> Get(DateOnly from, DateOnly to);
}
