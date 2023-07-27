
using TransactionsLog.Models.Entities;

namespace TransactionsLog.DataLayer.Abstractions.Repositories
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
        Task<IList<Transaction>> GetAllWithTransactionType();
    }
}
