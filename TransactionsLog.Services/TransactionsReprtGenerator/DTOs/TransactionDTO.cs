using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionsLog.Services.TransactionsReportGenerator.DTOs
{
    public class TransactionDTO
    {
        public string TransactionType { get; private set; }
        public DateTime Timestamp { get; private set; }
        public decimal Value { get; private set; }

        public TransactionDTO(string transactionType, DateTime timestamp, decimal value)
        {
            TransactionType = transactionType;
            Timestamp = timestamp;
            Value = value;
        }
        
    }
}
