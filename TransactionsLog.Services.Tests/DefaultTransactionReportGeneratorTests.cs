using FluentAssertions;
using Moq;
using TransactionsLog.DataLayer.Abstractions.Repositories;
using TransactionsLog.Models.Entities;
using TransactionsLog.Services.TransactionsReportGenerator;

namespace TransactionsLog.Services.Tests
{
    public class DefaultTransactionReportGeneratorTests
    {
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly DefaultTransactionReportGenerator _defaultTransactionReportGenerator;

        public DefaultTransactionReportGeneratorTests()
        {
            _transactionRepositoryMock = new();
            _defaultTransactionReportGenerator = new(_transactionRepositoryMock.Object);
        }

        [Fact]
        public async Task ShouldGroupTransactionsByStore()
        {
            var store1 = "Store1";
            var store2 = "Store2";
            var transactions = new[]
            {
                Mock.Of<Transaction>(t => t.StoreName == store1 && t.TransactionType == Mock.Of<TransactionType>()),
                Mock.Of<Transaction>(t => t.StoreName == store2 && t.TransactionType == Mock.Of<TransactionType>())
            };

            _transactionRepositoryMock.Setup(tr => tr.GetAllWithTransactionType()).ReturnsAsync(transactions);

            var report = await _defaultTransactionReportGenerator.Generate();

            report.Should().HaveCount(2);
        }

        [Fact]
        public async Task ShouldCalculateBalanceOfStore()
        {            
            var transactions = new[]
            {
                Mock.Of<Transaction>(t => t.StoreName == "store" && t.Value == 100 && t.TransactionType == Mock.Of<TransactionType>()),
                Mock.Of<Transaction>(t => t.StoreName == "store" && t.Value == -50 && t.TransactionType == Mock.Of<TransactionType>())
            };

            _transactionRepositoryMock.Setup(tr => tr.GetAllWithTransactionType()).ReturnsAsync(transactions);

            var report = await _defaultTransactionReportGenerator.Generate();

            report.Should().ContainSingle().Which.Balance.Should().Be(50);
        }
    }
}
