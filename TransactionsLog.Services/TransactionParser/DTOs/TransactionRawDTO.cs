using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionsLog.Services.TransactionParser.DTOs
{
    public class TransactionRawDTO
    {

        public int TransactionTypeId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal AbsoluteValue { get; set; }
        public string Cpf { get; set; }
        public string Card { get; set; }
        public string StoreOwnerName { get; set; }
        public string StoreName { get; set; }
    }
}
