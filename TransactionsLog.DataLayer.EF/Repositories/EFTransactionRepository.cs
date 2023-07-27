using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionsLog.DataLayer.Abstractions.Repositories;
using TransactionsLog.Models.Entities;

namespace TransactionsLog.DataLayer.EF.Repositories
{
    public class EFTransactionRepository : EFBaseRepository<Transaction>, ITransactionRepository
    {
        public EFTransactionRepository(TransactionsLogContext context) : base(context)
        {
        }

        public async Task<IList<Transaction>> GetAllWithTransactionType()
        {
            return await _context.Transactions.Include(t => t.TransactionType).ToListAsync();
        }
    }
}
