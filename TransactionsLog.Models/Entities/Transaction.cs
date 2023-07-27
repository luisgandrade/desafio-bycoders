using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionsLog.Models.Enums;

namespace TransactionsLog.Models.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Value { get; set; }
        public string Cpf { get; set; } = string.Empty;
        public string Card { get; set; } = string.Empty;
        public string StoreOwnerName { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public TransactionType TransactionType { get; set; }
    }
}
