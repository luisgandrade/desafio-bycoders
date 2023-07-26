using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionsLog.DataLayer.Abstractions.Repositories
{
    public interface IBaseRepository<TEntity>
        where TEntity : class
    {
        Task Insert(TEntity entity);
        Task Update(TEntity book);
        Task Delete(int id);
        Task<TEntity?> Get(int id);
        Task<IList<TEntity>> GetAll();
    }
}
