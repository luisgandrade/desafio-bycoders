using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionsLog.DataLayer.Abstractions;

namespace TransactionsLog.DataLayer.EF
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly TransactionsLogContext _context;
        private ILogger<EFUnitOfWork> _logger;

        public EFUnitOfWork(TransactionsLogContext context, ILogger<EFUnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Commit()
        {
            try
            {
                if (_context.ChangeTracker.HasChanges())
                {
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao commitar transação");
                throw;
            }


        }
    }
}
