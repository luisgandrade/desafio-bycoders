using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionsLog.Services.TransactionsLogger.DTOs
{
    public class LoggingResult
    {
        public bool Success { get; set; }

        public IList<string> ErrorMessages  { get; set; } = new List<string>();
    }
}
