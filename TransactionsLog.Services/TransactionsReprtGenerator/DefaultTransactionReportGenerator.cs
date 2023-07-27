using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionsLog.DataLayer.Abstractions.Repositories;
using TransactionsLog.Services.TransactionsReportGenerator.DTOs;

namespace TransactionsLog.Services.TransactionsReportGenerator
{
    public class DefaultTransactionReportGenerator : ITransactionsReportGenerator
    {
        private readonly ITransactionRepository _transactionRepository;

        public DefaultTransactionReportGenerator(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<IList<TransactionsOfStoreDTO>> Generate()
        {
            var allTransactions = await _transactionRepository.GetAllWithTransactionType();

            return allTransactions.GroupBy(t => t.StoreName)
                .Select(ts => new TransactionsOfStoreDTO(
                    storeName: ts.Key,
                    transactions: ts.Select(t => new TransactionDTO(t.TransactionType.Description, t.Timestamp, t.Value)).ToList())).ToList();
        }
    }
}
