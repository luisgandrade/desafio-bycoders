using TransactionsLog.Services.TransactionParser.DTOs;

namespace TransactionsLog.Services.TransactionParser
{
    public interface ITransactionParser
    {
        (TransactionRawDTO?, string) ParseRecord(string line);
    }
}
