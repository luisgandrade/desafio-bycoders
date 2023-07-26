using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionsLog.Models.Enums
{
    public enum TransactionFlow
    {
        Outbound,
        Inbound
    }

    public static class TransactionFlowExtensions
    {
        public static decimal Signal(this TransactionFlow transactionFlow)
        {
            return transactionFlow switch
            {
                TransactionFlow.Outbound => -1,
                TransactionFlow.Inbound => 1,
                _ => throw new InvalidOperationException("Unexpected value for TransactionFlow enum")
            };
        }
    }
}
