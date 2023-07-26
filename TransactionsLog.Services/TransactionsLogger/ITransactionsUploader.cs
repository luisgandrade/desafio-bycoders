using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionsLog.Services.TransactionsLogger.DTOs;

namespace TransactionsLog.Services.TransactionsLogger
{
    public interface ITransactionsLogger
    {

        Task<LoggingResult> FromFile(Stream file);
    }
}
