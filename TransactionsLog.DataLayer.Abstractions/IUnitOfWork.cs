using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionsLog.DataLayer.Abstractions
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}
