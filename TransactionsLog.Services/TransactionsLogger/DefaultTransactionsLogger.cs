using TransactionsLog.DataLayer.Abstractions;
using TransactionsLog.DataLayer.Abstractions.Repositories;
using TransactionsLog.Models.Entities;
using TransactionsLog.Models.Enums;
using TransactionsLog.Services.TransactionParser;
using TransactionsLog.Services.TransactionParser.DTOs;
using TransactionsLog.Services.TransactionsLogger;
using TransactionsLog.Services.TransactionsLogger.DTOs;

namespace TransactionsLog.Services.TransactionsUploader
{
    public class DefaultTransactionsLogger : ITransactionsLogger
    {
        private class RecordParsingResultsWithLine
        {
            public int Line { get; set; }
            public TransactionRawDTO? Transaction { get; set; }
            public string? ErrorMessage { get; set; }
        }

        private readonly IBaseRepository<Transaction> _transactionRepository;
        private readonly IBaseRepository<TransactionType> _transactionTypeRepository;
        private readonly ITransactionParser _transactionParser;
        private readonly IUnitOfWork _unitOfWork;

        public DefaultTransactionsLogger(IBaseRepository<Transaction> transactionRepository, 
            IBaseRepository<TransactionType> transactionTypeRepository, 
            ITransactionParser transactionParser, 
            IUnitOfWork unitOfWork)
        {
            _transactionRepository = transactionRepository;
            _transactionTypeRepository = transactionTypeRepository;
            _transactionParser = transactionParser;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoggingResult> FromFile(Stream file)
        {
            if (file is null)
                throw new ArgumentNullException(nameof(file));
            if (!file.CanRead)
                throw new InvalidOperationException("Stream de arquivo já foi fechado");

            var lineParsingResults = new List<RecordParsingResultsWithLine>();

            using (var streamReader = new StreamReader(file))
            {
                var line = 1;
                while (!streamReader.EndOfStream)
                {
                    var nextLine = await streamReader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(nextLine))
                    {
                        line++;
                        continue;
                    }
                    var parsingResults = _transactionParser.ParseRecord(nextLine);
                    lineParsingResults.Add(new RecordParsingResultsWithLine { Line = line, Transaction = parsingResults.Item1, ErrorMessage = parsingResults.Item2 });
                    line++;
                }
            }

            var parsingResultsWithError = lineParsingResults.Where(lpr => lpr.Transaction is null).ToList();
            if (parsingResultsWithError.Any())
            {
                return new LoggingResult
                {
                    Success = false,
                    ErrorMessages = parsingResultsWithError.Select(pre => $"Linha {pre.Line}: {pre.ErrorMessage}").ToList()
                };
            }

            var transactionTypes = await _transactionTypeRepository.GetAll();
            var parsingResultWithTransactionType = lineParsingResults.GroupJoin(transactionTypes, pr => pr.Transaction.TransactionTypeId, tt => tt.Id, (pr, tt) => new
            {
                parsingResult = pr,
                transactionType = tt.SingleOrDefault()
            }).ToList();

            var parsingResultsWithInvalidTransactionTypes = parsingResultWithTransactionType.Where(prt => prt.transactionType is null).ToList();
            if (parsingResultsWithInvalidTransactionTypes.Any())
            {
                return new LoggingResult
                {
                    Success = false,
                    ErrorMessages = parsingResultsWithInvalidTransactionTypes.Select(pre => $"Linha {pre.parsingResult.Line}: Tipo de transação desconhecido").ToList()
                };
            }

            foreach (var parsingResult in parsingResultWithTransactionType)
            {
                var transaction = new Transaction
                {                    
                    Card = parsingResult.parsingResult.Transaction.Card,
                    Cpf = parsingResult.parsingResult.Transaction.Cpf,
                    StoreName = parsingResult.parsingResult.Transaction.StoreName,
                    StoreOwnerName = parsingResult.parsingResult.Transaction.StoreOwnerName,
                    TransactionType = parsingResult.transactionType,
                    Timestamp = parsingResult.parsingResult.Transaction.Timestamp,
                    Value = parsingResult.transactionType.TransactionFlow.Signal() * parsingResult.parsingResult.Transaction.AbsoluteValue
                };

                await _transactionRepository.Insert(transaction);
            }

            await _unitOfWork.Commit();

            return new LoggingResult { Success = true };

        }
    }
}
