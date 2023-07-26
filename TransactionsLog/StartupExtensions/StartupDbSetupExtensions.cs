using Microsoft.EntityFrameworkCore;
using TransactionsLog.DataLayer.EF;
using TransactionsLog.Models.Entities;
using TransactionsLog.Models.Enums;

namespace TransactionsLog.StartupExtensions
{
    public static class StartupDbSetupExtensions
    {

        private static IList<TransactionType> CreateBaseTransactionTypes() => new[]
        {
            new TransactionType { Id = 1, Description = "Débito", TransactionFlow = TransactionFlow.Inbound },
            new TransactionType { Id = 2, Description = "Boleto", TransactionFlow = TransactionFlow.Outbound },
            new TransactionType { Id = 3, Description = "Financiamento", TransactionFlow = TransactionFlow.Outbound },
            new TransactionType { Id = 4, Description = "Crédito", TransactionFlow = TransactionFlow.Inbound },
            new TransactionType { Id = 5, Description = "Recebimento Empréstimo", TransactionFlow = TransactionFlow.Inbound },
            new TransactionType { Id = 6, Description = "Vendas", TransactionFlow = TransactionFlow.Inbound },
            new TransactionType { Id = 7, Description = "Recebimento TED", TransactionFlow = TransactionFlow.Inbound },
            new TransactionType { Id = 8, Description = "Recebimento DOC", TransactionFlow = TransactionFlow.Inbound },
            new TransactionType { Id = 9, Description = "Aluguel", TransactionFlow = TransactionFlow.Outbound },
        };

        public static IApplicationBuilder BootstrapDb(this IApplicationBuilder applicationBuilder)
        {
            using(var scope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<TransactionsLogContext>() ?? throw new InvalidOperationException("DbContext not configured");

                if (dbContext.Database.EnsureCreated())
                {
                    var transactionTypes = CreateBaseTransactionTypes();
                    dbContext.TransactionTypes.AddRange(transactionTypes);
                    dbContext.SaveChanges();
                }                
            }
            
            return applicationBuilder;
        }
    }
}
