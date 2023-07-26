using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionsLog.Models.Enums;

namespace TransactionsLog.Models.Entities
{
    public class TransactionType
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public TransactionFlow TransactionFlow { get; set; }

    }
}
