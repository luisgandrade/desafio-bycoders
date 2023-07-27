using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionsLog.Services.TransactionsReportGenerator.DTOs
{
    public class TransactionsOfStoreDTO
    {
        public string StoreName { get; private set; }
        public IList<TransactionDTO> Transactions { get; private set; }
        public decimal Balance => Transactions.Sum(t => t.Value);

        public TransactionsOfStoreDTO(string storeName, IList<TransactionDTO> transactions)
        {
            StoreName = storeName;
            Transactions = transactions;
        }
    }
}
