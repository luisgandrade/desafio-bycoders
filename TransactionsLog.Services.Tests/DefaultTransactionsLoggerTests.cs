using FluentAssertions;
using Moq;
using System.Text;
using TransactionsLog.DataLayer.Abstractions;
using TransactionsLog.DataLayer.Abstractions.Repositories;
using TransactionsLog.Models.Entities;
using TransactionsLog.Services.TransactionParser;
using TransactionsLog.Services.TransactionParser.DTOs;
using TransactionsLog.Services.TransactionsUploader;

namespace TransactionsLog.Services.Tests
{
    public class DefaultTransactionsLoggerTests
    {

        private readonly Mock<IBaseRepository<Transaction>> _transactionRepositoryMock;
        private readonly Mock<IBaseRepository<TransactionType>> _transactionTypeRepositoryMock;
        private readonly Mock<ITransactionParser> _transactionParserMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DefaultTransactionsLogger _transactionLogger;

        public DefaultTransactionsLoggerTests()
        {
            _transactionRepositoryMock = new();
            _transactionTypeRepositoryMock = new();
            _transactionParserMock = new();
            _unitOfWorkMock = new();
            _transactionLogger = new(_transactionRepositoryMock.Object, _transactionTypeRepositoryMock.Object, _transactionParserMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]

        public async Task ShoudlThrowAnExceptionIfInputStreamIsInvalid()
        {
            Stream inputStream = null;

            var loggingAction = () => _transactionLogger.FromFile(inputStream);

            await loggingAction.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]

        public async Task ShoudlThrowAnExceptionIfInputStreamIsClosed()
        {
            var inputStream = new MemoryStream();
            inputStream.Close();

            var loggingAction = async () => await _transactionLogger.FromFile(inputStream);

            await loggingAction.Should().ThrowAsync<InvalidOperationException>();
        }


        [Fact]

        public async Task ShoudlFailIfParsingSomeLineFailed()
        {
            var firstRecord = "askjoaskosajas";
            var secondRecord = "sdajiaisjiasojsa";
            (TransactionRawDTO?, string) firstRecordParseResult = (null, "errroror");
            (TransactionRawDTO?, string) secondRecordParseResult = (new TransactionRawDTO { }, string.Empty);
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes($"{firstRecord}\n{secondRecord}"));

            _transactionParserMock.Setup(tp => tp.ParseRecord(firstRecord)).Returns(firstRecordParseResult);
            _transactionParserMock.Setup(tp => tp.ParseRecord(secondRecord)).Returns(secondRecordParseResult);


            var loggingResult = await _transactionLogger.FromFile(inputStream);

            loggingResult.Success.Should().BeFalse();
            loggingResult.ErrorMessages.Single().Should().Be($"Linha 1: {firstRecordParseResult.Item2}");
            _transactionRepositoryMock.Verify(tr => tr.Insert(It.IsAny<Transaction>()), Times.Never());
            _unitOfWorkMock.Verify(tr => tr.Commit(), Times.Never());
        }

        [Fact]

        public async Task ShoudlFailIfTheTransactionTypeOfSomeRecordIsUnknown()
        {
            var record = "askjoaskosajas";
            (TransactionRawDTO?, string) parseResult = (new TransactionRawDTO { TransactionTypeId = 3 }, string.Empty);
            var transactionTypes = new[]
            {
                Mock.Of<TransactionType>(tt => tt.Id == 1),
                Mock.Of<TransactionType>(tt => tt.Id == 2)
            };
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(record));

            _transactionParserMock.Setup(tp => tp.ParseRecord(record)).Returns(parseResult);
            _transactionTypeRepositoryMock.Setup(ttr => ttr.GetAll()).ReturnsAsync(transactionTypes);

            var loggingResult = await _transactionLogger.FromFile(inputStream);

            loggingResult.Success.Should().BeFalse();
            loggingResult.ErrorMessages.Single().Should().Be($"Linha 1: Tipo de transação desconhecido");
            _transactionRepositoryMock.Verify(tr => tr.Insert(It.IsAny<Transaction>()), Times.Never());
            _unitOfWorkMock.Verify(tr => tr.Commit(), Times.Never());
        }

        [Fact]

        public async Task ShoudlWriteTransactionToRepositoryIfItIsEverythingOk()
        {
            var record = "askjoaskosajas";
            var transactionRawDto = new TransactionRawDTO
            {
                TransactionTypeId = 1,
                AbsoluteValue = 10,
                Card = "kjasokas",
                Cpf = "1111111",
                StoreName = "koasksa",
                StoreOwnerName = "jsiajsai",
                Timestamp = DateTime.Now
            };
            (TransactionRawDTO?, string) parseResult = (transactionRawDto, string.Empty);
            var transactionTypes = new[]
            {
                Mock.Of<TransactionType>(tt => tt.Id == 1 && tt.TransactionFlow == Models.Enums.TransactionFlow.Inbound),
                Mock.Of<TransactionType>(tt => tt.Id == 2 && tt.TransactionFlow == Models.Enums.TransactionFlow.Inbound)
            };
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(record));

            _transactionParserMock.Setup(tp => tp.ParseRecord(record)).Returns(parseResult);
            _transactionTypeRepositoryMock.Setup(ttr => ttr.GetAll()).ReturnsAsync(transactionTypes);

            var loggingResult = await _transactionLogger.FromFile(inputStream);

            Func<Transaction, bool> matchesTestTransaction = transaction =>
                transaction.Value == transactionRawDto.AbsoluteValue &&
                transaction.Card == transactionRawDto.Card &&
                transaction.Cpf == transactionRawDto.Cpf &&
                transaction.StoreName == transactionRawDto.StoreName &&
                transaction.StoreOwnerName == transactionRawDto.StoreOwnerName &&
                transaction.Timestamp == transactionRawDto.Timestamp &&
                transaction.TransactionType.Id == transactionRawDto.TransactionTypeId;

            loggingResult.Success.Should().BeTrue();
            _transactionRepositoryMock.Verify(tr => tr.Insert(It.Is<Transaction>(t => matchesTestTransaction(t))), Times.Once());
            _unitOfWorkMock.Verify(tr => tr.Commit(), Times.Once());
        }

        [Fact]

        public async Task ShoudlWriteTransactionWithNegativeValueIfTransationTypeIsOutbound()
        {
            var record = "askjoaskosajas";
            var transactionRawDto = new TransactionRawDTO
            {
                TransactionTypeId = 1,
                AbsoluteValue = 10,
                Card = "kjasokas",
                Cpf = "1111111",
                StoreName = "koasksa",
                StoreOwnerName = "jsiajsai",
                Timestamp = DateTime.Now
            };
            (TransactionRawDTO?, string) parseResult = (transactionRawDto, string.Empty);
            var transactionTypes = new[]
            {
                Mock.Of<TransactionType>(tt => tt.Id == 1 && tt.TransactionFlow == Models.Enums.TransactionFlow.Outbound)
            };
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(record));

            _transactionParserMock.Setup(tp => tp.ParseRecord(record)).Returns(parseResult);
            _transactionTypeRepositoryMock.Setup(ttr => ttr.GetAll()).ReturnsAsync(transactionTypes);

            var loggingResult = await _transactionLogger.FromFile(inputStream);

            loggingResult.Success.Should().BeTrue();
            _transactionRepositoryMock.Verify(tr => tr.Insert(It.Is<Transaction>(t => t.Value == transactionRawDto.AbsoluteValue * -1)), Times.Once());
            _unitOfWorkMock.Verify(tr => tr.Commit(), Times.Once());
        }

    }
}
