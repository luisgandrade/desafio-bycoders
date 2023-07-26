using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionsLog.Models.Entities;
using TransactionsLog.Models.Enums;

namespace TransactionsLog.DataLayer.EF
{
    public class TransactionsLogContext : DbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }

        public TransactionsLogContext(DbContextOptions<TransactionsLogContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TransactionType>()
                .Property(tt => tt.TransactionFlow)
                .HasConversion(vFromApp => (int)vFromApp, vFromDb => (TransactionFlow)vFromDb);

            modelBuilder.Entity<Transaction>()
                .Ignore(t => t.Value);
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.TransactionType).WithMany().IsRequired();
        }
    }
}
