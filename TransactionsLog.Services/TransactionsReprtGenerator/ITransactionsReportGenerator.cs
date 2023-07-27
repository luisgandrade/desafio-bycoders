using TransactionsLog.Services.TransactionsReportGenerator.DTOs;

namespace TransactionsLog.Services.TransactionsReportGenerator
{
    public interface ITransactionsReportGenerator
    {

        Task<IList<TransactionsOfStoreDTO>> Generate();
    }
}
