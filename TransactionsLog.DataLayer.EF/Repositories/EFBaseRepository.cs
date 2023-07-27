using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionsLog.DataLayer.Abstractions.Repositories;

namespace TransactionsLog.DataLayer.EF.Repositories
{
    public class EFBaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
        protected readonly TransactionsLogContext _context;

        public EFBaseRepository(TransactionsLogContext context)
        {
            _context = context;
        }

        public async Task Delete(int id)
        {
            var entityFound = await Get(id);
            if (entityFound is not null)
                _context.Remove(entityFound);
        }

        public async Task<TEntity?> Get(int id)
        {
            return await _context.FindAsync<TEntity>(id);
        }

        public async Task<IList<TEntity>> GetAll()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task Insert(TEntity entity)
        {
            await _context.AddAsync<TEntity>(entity);
        }

        public Task Update(TEntity book)
        {
            //Update não faz sentido para o EF se o Change Tracker estiver ativado.
            //O método foi adicionado para atender a interface
            return Task.CompletedTask;
        }
    }
}
